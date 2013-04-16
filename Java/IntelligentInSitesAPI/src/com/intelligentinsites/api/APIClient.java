package com.intelligentinsites.api;

import org.apache.commons.codec.binary.Base64;
import org.apache.commons.io.IOUtils;
import org.apache.http.HttpHost;
import org.apache.http.HttpResponse;
import org.apache.http.NameValuePair;
import org.apache.http.StatusLine;
import org.apache.http.auth.AuthScope;
import org.apache.http.auth.UsernamePasswordCredentials;
import org.apache.http.client.entity.UrlEncodedFormEntity;
import org.apache.http.entity.mime.MultipartEntity;
import org.apache.http.entity.mime.content.FileBody;
import org.apache.http.entity.mime.content.StringBody;
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.client.params.ClientPNames;
import org.apache.http.client.utils.URLEncodedUtils;
import org.apache.http.impl.client.BasicCookieStore;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.params.HttpProtocolParams;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.nio.charset.Charset;
import java.util.List;
import java.util.Map.Entry;

/**
 * A sample HTTP client class using the Apache HttpComponents libraries. This
 * class simplifies HTTP GET and POST requests against the Intelligent InSites 
 * API. Credentials are submitted using basic access authentication.
 */
public class APIClient {
    private String authHeaderValue = null;
    private DefaultHttpClient client = null;
    
    public enum URIScheme {
    	HTTP("http"),
    	HTTPS("https");
    	
    	private String name;
    	
    	private URIScheme(String name) {
    		this.name = name;
    	}
    	
    	public String getName() {
    		return name;
    	}
    }

    
    /**
     * APIClient constructor using HTTPS over port 443.
     * 
     * @param host		the hostname of the InSites server
     * @param username	a username associated with an InSites login
     * @param password	the password of the specified user
     */
    public APIClient(String host, String username, String password) {
    	this(URIScheme.HTTPS, host, username, password, 443);
    }
    
    /**
     * APIClient constructor.
     * 
     * @param scheme	the URI scheme to use in API requests
     * @param host		the hostname of the InSites server
     * @param username	a username associated with an InSites login
     * @param password	the password of the specified user
     * @param port		the port number used to connect to the API service
     */
    public APIClient(URIScheme scheme, String host, String username, String password, int port) {
        client = new DefaultHttpClient();
        client.getParams().setParameter(ClientPNames.DEFAULT_HOST, new HttpHost(host, port, scheme.name));
        HttpProtocolParams.setUserAgent(client.getParams(), "InSites Java Connection");
        client.setCookieStore(new BasicCookieStore());
        client.getCredentialsProvider().setCredentials(new AuthScope(host, port), new UsernamePasswordCredentials(username, password));
        authHeaderValue = "Basic " + Base64.encodeBase64String((username + ":" + password).getBytes());
    }

    /**
     * Performs an HTTP GET request.
     * Example: <code>get("/api/2.0/rest/equipment.xml?select=name");</code>
     * 
     * @param uri	the path and query string portions of a resource URI
     * @return		the GET response as a String
     */
    public String get(String uri) {
        HttpGet request = new HttpGet(uri);
        return execute(request);
    }
    
    /**
     * Performs an HTTP GET request.
     * Example: <code>get("/api/2.0/rest/equipment.xml");</code>
     * 
     * @param uri			the path portion of a resource URI
     * @param parameters	the parameters to pass with the request
     * @return				the GET response as a String
     */
    public String get(String uri, APIParams parameters) {
        List<NameValuePair> qparams = parameters.getStringParams();
        HttpGet request = new HttpGet(uri + "?" + URLEncodedUtils.format(qparams, "UTF-8"));
        return execute(request);
    }

    /**
     * Performs an HTTP POST request.
     * Example: <code>post("/api/2.0/rest/sensors/Bxrx/button-press.xml?button=2");</code>
     * 
     * @param uri	the path and query string portions of a resource URI
     * @return		the POST response as a String
     */
    public String post(String uri) {
        HttpPost request = new HttpPost(uri);
        return execute(request);
    }
    
    /**
     * Performs an HTTP POST request.
     * 
     * @param uri			the path portion of a resource URI
     * @param parameters	the parameters to pass with the request
     * @return				the POST response as a String
     */
    public String post(String uri, APIParams parameters) {
        HttpPost request = new HttpPost(uri);

        List<NameValuePair> formParams = parameters.getStringParams();
        UrlEncodedFormEntity entity = null;
        try {
            entity = new UrlEncodedFormEntity(formParams, "UTF-8");
        } catch (UnsupportedEncodingException e) {
            throw new RuntimeException(e);
        }
        request.setEntity(entity);

        return execute(request);
    }
    
    /**
     * Performs an HTTP POST request using the multipart/form-data method.
     * 
     * @param uri			the path portion of a resource URI
     * @param parameters	the parameters to pass with the request
     * @return				the POST response as a String
     */
    public String postMultipart(String uri, APIParams parameters) {
        HttpPost request = new HttpPost(uri);

        List<NameValuePair> stringParams = parameters.getStringParams();
        List<Entry<String, File>> fileParams = parameters.getFileParams();
        
        MultipartEntity entity = null;
        try {
            entity = new MultipartEntity();
            for(NameValuePair param : stringParams) {
            	entity.addPart(param.getName(), new StringBody(param.getValue(), Charset.forName("UTF-8")));
            }
            for(Entry<String, File> param : fileParams) {
            	entity.addPart((String)param.getKey(), new FileBody(param.getValue()));
            }
        } catch (UnsupportedEncodingException e) {
            throw new RuntimeException(e);
        }
        request.setEntity(entity);

        return execute(request);
    }

    /**
     * Submits an HTTP request.
     * 
     * @param request	the HttpUriRequest to submit
     * @return			the String response
     */
    private String execute(HttpUriRequest request) throws RuntimeException {
        request.addHeader("Authorization", authHeaderValue);
        try {
            HttpResponse response = client.execute(request);
            StatusLine status = response.getStatusLine();
            if (status.getStatusCode() != 200) {
                request.abort();
                throw new RuntimeException("Error executing request: " + status.getStatusCode() + " - " + status.toString());
            }
            InputStream instream = response.getEntity().getContent();
            
            try {
                return inputStreamToString(instream);
                // do something useful with the response
            }
            catch (RuntimeException ex) {
                // In case of an unexpected exception you may want to abort
                // the HTTP request in order to shut down the underlying
                // connection immediately.
                request.abort();
                throw ex;
            }
            finally {
                // Closing the input stream will trigger a connection release
                try {
                    if (instream != null) {
                        instream.close();
                    }
                }
                catch (Exception ignore) {
                }
            }
        }
        catch (IOException e) {
            throw new RuntimeException(e);
        }
    }
    
    /**
     * Consumes an InputStream, and builds a String representation of the data.
     * 
     * @param in	an InputStream to convert
     * @return		the String representation of the input bytes
     */
    private String inputStreamToString(InputStream in) {
        StringWriter writer = new StringWriter();
        try {
            IOUtils.copy(in, writer, "UTF-8");
        } catch (IOException e) {
            throw new RuntimeException(e);
        }
        return writer.toString();
    }
}