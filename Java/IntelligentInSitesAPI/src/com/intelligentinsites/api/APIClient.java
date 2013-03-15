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
import org.apache.http.client.methods.HttpGet;
import org.apache.http.client.methods.HttpPost;
import org.apache.http.client.methods.HttpUriRequest;
import org.apache.http.client.params.ClientPNames;
import org.apache.http.client.utils.URLEncodedUtils;
import org.apache.http.impl.client.BasicCookieStore;
import org.apache.http.impl.client.DefaultHttpClient;
import org.apache.http.message.BasicNameValuePair;
import org.apache.http.params.HttpProtocolParams;

import java.io.IOException;
import java.io.InputStream;
import java.io.StringWriter;
import java.io.UnsupportedEncodingException;
import java.util.ArrayList;
import java.util.Collection;
import java.util.List;
import java.util.Map;


/**
 * A sample HTTP client class using the Apache HttpComponents libraries. This
 * class simplifies HTTP GET and POST requests against the Intelligent InSites 
 * API. Credentials are submitted using basic access authentication.
 */
public class APIClient {
    private String authHeaderValue = null;
    private DefaultHttpClient client = null;
    private int port = 80;
    private String scheme = "http";

    
    /**
     * APIClient constructor using HTTP over port 80.
     * 
     * @param host		the hostname of the InSites server
     * @param username	a username associated with an InSites login
     * @param password	the password of the specified user
     */
    public APIClient(String host, String username, String password) {
    	this(host, username, password, false);
    }
    
    /**
     * APIClient constructor.
     * 
     * @param host		the hostname of the InSites server
     * @param username	a username associated with an InSites login
     * @param password	the password of the specified user
     * @param useSSL	true to use HTTPS over port 443. By default APIClient will use HTTP over port 80.
     */
    public APIClient(String host, String username, String password, boolean useSSL) {
    	if (useSSL) {
    		this.scheme = "https";
    		this.port = 443;
    	}
        client = new DefaultHttpClient();
        client.getParams().setParameter(ClientPNames.DEFAULT_HOST, new HttpHost(host, port, scheme));
        HttpProtocolParams.setUserAgent(client.getParams(), "InSites Java Connection");
        client.setCookieStore(new BasicCookieStore());

        client.getCredentialsProvider().setCredentials(new AuthScope(host, 80), new UsernamePasswordCredentials(username, password));
        client.getCredentialsProvider().setCredentials(new AuthScope(host, 443), new UsernamePasswordCredentials(username, password));
        authHeaderValue = "Basic " + Base64.encodeBase64String((username + ":" + password).getBytes());
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

    /**
     * Performs an HTTP GET request.
     * 
     * @param url	the url of the requested resource
     * @return		the String response
     */
    public String get(String url) {
        HttpGet request = new HttpGet(url);
        return execute(request);
    }
    
    /**
     * Performs an HTTP GET request.
     * 
     * @param url			the url of the requested resource
     * @param parameters	a Map of parameter names and values
     * @return				the String response
     */
    public String get(String url, Map<String, Object> parameters) {
        List<NameValuePair> qparams = assembleParameters(parameters);
        HttpGet request = new HttpGet(url + "?" + URLEncodedUtils.format(qparams, "UTF-8"));
        return execute(request);
    }

    /**
     * Performs an HTTP POST request.
     * 
     * @param url	the url of the requested resource
     * @return		the String response
     */
    public String post(String url) {
        HttpPost request = new HttpPost(url);
        return execute(request);
    }
    
    /**
     * Performs an HTTP POST request.
     * 
     * @param url			the url of the requested resource
     * @param parameters	a Map of parameter names and values
     * @return				the String response
     */
    public String post(String url, Map<String, Object> parameters) {
        HttpPost request = new HttpPost(url);

        List<NameValuePair> formParams = assembleParameters(parameters);
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
     * Converts a Map of parameters into a List of {@link NameValuePair} objects.
     * 
     * @param parameters	a Map of parameter names and values
     * @return				a List of parameter names and values represented as NameValuePair objects
     */
    private List<NameValuePair> assembleParameters(Map<String, Object> parameters) {
        List<NameValuePair> formParams = new ArrayList<NameValuePair>();
        for (Map.Entry<String, Object> param : parameters.entrySet()) {
            Object value = param.getValue();
            if (value instanceof Collection) {
                for (Object o : (Collection) value) {
                    formParams.add(new BasicNameValuePair(param.getKey(), o.toString()));
                }
            } else {
                formParams.add(new BasicNameValuePair(param.getKey(), value.toString()));
            }
        }
        return formParams;
    }
}