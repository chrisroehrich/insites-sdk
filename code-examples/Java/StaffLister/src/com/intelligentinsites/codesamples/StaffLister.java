package com.intelligentinsites.codesamples;

import java.io.ByteArrayInputStream;
import java.io.ByteArrayOutputStream;
import java.io.IOException;
import java.io.PrintWriter;
import java.util.HashMap;
import java.util.Map;

import javax.servlet.ServletException;
import javax.servlet.http.HttpServlet;
import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.transform.Transformer;
import javax.xml.transform.TransformerFactory;
import javax.xml.transform.stream.StreamSource;
import javax.xml.transform.stream.StreamResult;
import javax.xml.xpath.XPathConstants;
import javax.xml.xpath.XPathExpression;
import javax.xml.xpath.XPathFactory;

import org.w3c.dom.Document;
import org.w3c.dom.NodeList;

import com.intelligentinsites.codesamples.BasicClient;

/**
 * Servlet implementation class StaffLister
 */
public class StaffLister extends HttpServlet {
	private final int resultsPerPage = 5;
	private static final long serialVersionUID = 1L;
	BasicClient inSitesConnection;

    /**
     * Default constructor. 
     */
    public StaffLister() {
        inSitesConnection = new BasicClient("insites-dev.intelligentinsites.net", "username", "password");
    }

	protected void doGet(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		String location = request.getParameter("location");
		int pageNumber = Integer.parseInt(request.getParameter("page"));

		PrintWriter out = response.getWriter();
		
		//construct HTTP request
		Map<String, Object> getParams = new HashMap<String, Object>();
		getParams.put("filter", "current-location in '" + location + "'");
		getParams.put("first-result", (pageNumber - 1) * resultsPerPage);
		getParams.put("limit", resultsPerPage);
		getParams.put("sort", "name");
		getParams.put("select", "current-location.name,name,status.name,type.name");
        
		String inSitesResponseBody = inSitesConnection.get("/api/2.0/rest/staff.xml", getParams);
		
		//transform the response into an HTML table
		out.println(transformXML(inSitesResponseBody, "StaffTable.xslt").getOutputStream());
		
		//retrieve the total number of staff within the selected location
		int staffCount = Integer.parseInt(selectNodesFromXML(inSitesResponseBody, "/list-response/@total-count").item(0).getTextContent());
		
		//print the page links
		out.println(generatePageLinks(pageNumber, (staffCount + resultsPerPage - 1) / resultsPerPage, location));
	}

	protected void doPost(HttpServletRequest request, HttpServletResponse response) throws ServletException, IOException {
		PrintWriter out = response.getWriter();
		
		//retrieve a list of all locations that are floors
		String inSitesResponseBody = inSitesConnection.get("/api/2.0/rest/locations.xml?filter=location-level.name+eq+'Floor'&select=name&limit=-1");
		
		//generate a drop-down list of all floors
		out.println(transformXML(inSitesResponseBody, "LocationList.xslt").getOutputStream());
	}
    
    /**
     * Selects a set of nodes from xml based on an XPath expression.
     * 
     * @param xml	the source xml document
     * @param xpath	an xpath expression
     * @return		the set of nodes matching the xpath expression
     */
    private NodeList selectNodesFromXML(String xml, String xpath) {
        try {
            DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();
            Document doc = docBuilder.parse(new ByteArrayInputStream(xml.getBytes()));
            
            XPathFactory factory = XPathFactory.newInstance();
            XPathExpression expr = factory.newXPath().compile(xpath);

            return (NodeList) expr.evaluate(doc, XPathConstants.NODESET);

        }
        catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
    
    /**
     * Generates links to the paginated staff results.
     * 
     * @param currentPage	the number of the current page in the table to display
     * @param totalPages	the total number of pages
     * @param locationId	the id of the selected location
     * @return				an HTML snippet containing links to each page
     */
    private String generatePageLinks(int currentPage, int totalPages, String locationId) {
    	String result = "";
    	if (currentPage == 1)
			result += "<ul id=\"pagination\"><li class=\"previous-disabled\"><a href=\"#\">Previous</a></li>";
		else
			result += "<ul id=\"pagination\"><li class=\"previous\"><a href=\"#\" onclick=\"updateTable(" + (currentPage - 1) + ",'" + locationId + "')\">Previous</a></li>";
		
		for (int i = 0; i < totalPages; i++) {
			if (i + 1 == currentPage) {
				result += "<li class=\"active\"><a href=\"#\">" + (i + 1) + "</a></li>"; }
			else
				result += "<li><a href=\"#\" onclick=\"updateTable(" + (i + 1) + ",'" + locationId + "')\">" + (i + 1) + "</a></li>";
		}
		
		if (currentPage == totalPages)
			result += "<li class=\"next-disabled\"><a href=\"#\">Next</a></li></ul>";
		else
			result += "<li class=\"next\"><a href=\"#\" onclick=\"updateTable(" + (currentPage + 1) + ",'" + locationId + "')\">Next</a></li></ul>";
		
		return result;
    }
    
    /**
     * Performs an XSL transformation on the given xml
     * 
     * @param xml			the xml to transform
     * @param stylesheet	the relative path to an XSLT file
     * @return				a StreamResult holding the result of the transformation
     */
    private StreamResult transformXML(String xml, String stylesheet) {
    	StreamSource xmlSource = new StreamSource(new ByteArrayInputStream(xml.getBytes()));
		
		ByteArrayOutputStream transformResult = new ByteArrayOutputStream();
		StreamResult result = new StreamResult(transformResult);
		try {
			StreamSource stylesource = new StreamSource(stylesheet);

			TransformerFactory factory = TransformerFactory.newInstance();
			Transformer transformer = factory.newTransformer(stylesource);
			
			transformer.transform(xmlSource, result);
		}
		catch (Exception e) {
			System.out.println(e.getMessage());
			e.printStackTrace();
		}
		
		return result;
    }

}
