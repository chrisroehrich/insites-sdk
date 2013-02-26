
/*Sample Equipment Creator 

This simple console application demonstrates using the Intelligent InSites Web 
Services API to create new equipment from data entered into a command line. 
This class implements the APIClient class to perform HTTP requests.*/

package com.intelligentinsites.codesamples;

import com.intelligentinsites.api.APIClient;

import org.w3c.dom.Document;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.xpath.*;
import java.io.ByteArrayInputStream;
import java.util.HashMap;
import java.util.Map;
import java.util.Scanner;

public class EquipmentCreator {
    public static void main(String[] args) {
        APIClient client = new APIClient("insites-dev.intelligentinsites.net", "username", "password");

        System.out.println("Creating a new equipment resource...");

        //Get the required parameters from user
        Scanner scanner = new Scanner(System.in);
        System.out.print("name <Wheelchair 1>: ");
        String name = scanner.nextLine().trim();

        System.out.print("short-name <wc1>: ");
        String shortName = scanner.nextLine().trim();

        System.out.print("type <Bxc6x>: ");
        String type = scanner.nextLine().trim();

        System.out.print("status <Bxc>: ");
        String status = scanner.nextLine().trim();

        System.out.print("service-status <Bxc>: ");
        String serviceStatus = scanner.nextLine().trim();

        //Build a Map of parameters
        Map<String, Object> createParams = new HashMap<String, Object>();
        createParams.put("name", name);
        createParams.put("shortName", shortName);
        createParams.put("type", type);
        createParams.put("status", status);
        createParams.put("serviceStatus", serviceStatus);
        
        //Perform a POST request to create a resource
        String createResponse = client.post("/api/2.0/rest/equipment.xml", createParams);
        System.out.println("Created a new equipment resource. The web service responded with:");
        System.out.println(createResponse);

        //Parse the response to get the id of the new equipment
        String equipmentId = parseEquipmentIdFromXMLResponse(createResponse);
        System.out.println("Parsed the id from the response. The id is: " + equipmentId);

        //Assign a sensor to the equipment by using the take-sensor method. Sensor resources are identified by 
        //two fields, a key (the 'provider' field), and a value (the 'label' field).
        System.out.println("Assigning an existing sensor to our new equipment...");
        System.out.print("Sensor to assign <provider.label>: ");
        String sensorId = scanner.nextLine().trim();

        Map<String, Object> assignParams = new HashMap<String, Object>();
        assignParams.put("sensor", sensorId);
        
        String assignResponse = client.post("/api/2.0/rest/equipment/"+equipmentId+"/take-sensor.xml", assignParams);
        System.out.println("Assigned a sensor to the new equipment. The web service responded with:");
        System.out.println(assignResponse);
        
        //Retrieve our new equipment with the sensor attached
        String getResponse = client.get("/api/2.0/rest/equipment/"+equipmentId+".xml");
        System.out.println("Retrieved our new equipment resource. The web service responded with:");
        System.out.println(getResponse);
        
        System.out.println("Press any key to quit");
        scanner.next();
        scanner.close();
    }
    
    /**
     * Retrieves the id of a newly created equipment resource from an InSites success response.
     * 
     * @param responseBody	the body text of an xml response
     * @return				the InSites id of the equipment
     */
    public static String parseEquipmentIdFromXMLResponse(String responseBody) {
    	String equipmentId = parseValueFromXML(responseBody, "//updated-resources/value[@element-type='equipment' and @provider='core']/@id");
    	if (!equipmentId.equals("")) {
    		return equipmentId;
    	}
		throw new RuntimeException("Equipment id not found!\n"+responseBody);
    }
    
    /**
     * Parses a string value from xml based on an XPath expression.
     * 
     * @param xml	the source xml document
     * @param xpath	an xpath expression
     * @return		the string value 
     */
    public static String parseValueFromXML(String xml, String xpath) {
        try {
            DocumentBuilderFactory docBuilderFactory = DocumentBuilderFactory.newInstance();
            DocumentBuilder docBuilder = docBuilderFactory.newDocumentBuilder();
            Document doc = docBuilder.parse(new ByteArrayInputStream(xml.getBytes()));
            
            XPathFactory factory = XPathFactory.newInstance();
            XPathExpression expr = factory.newXPath().compile(xpath);

            return expr.evaluate(doc);

        }
        catch (Exception e) {
            throw new RuntimeException(e);
        }
    }
}