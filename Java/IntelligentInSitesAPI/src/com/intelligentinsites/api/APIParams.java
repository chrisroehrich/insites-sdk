package com.intelligentinsites.api;

import java.io.File;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;
import java.util.Map.Entry;
import java.util.Set;
import java.util.AbstractMap.SimpleEntry;

import org.apache.http.NameValuePair;
import org.apache.http.message.BasicNameValuePair;

public class APIParams {
	private Map<String, List<Object>> params;
	
	public APIParams() {
		params = new HashMap<String, List<Object>>();
	}
	
	/**
	 * Adds a File parameter to the collection. This would typically be used in an HTTP multipart/form-data POST request.
	 * 
	 * @param name	the parameter name
	 * @param value	the File to add as a parameter
	 */
	public void add(String name, File value) {
		addObject(name, value);
	}
	
	/**
	 * Adds a String parameter to the collection.
	 * 
	 * @param name	the parameter name
	 * @param value	the value of the parameter
	 */
	public void add(String name, String value) {
		addObject(name, value);
	}
	
	/**
	 * Removes all parameters from the collection.
	 */
	public void clear() {
		params.clear();
	}
	
	/**
	 * Returns the names of all parameters in the collection.
	 * 
	 * @return	a {@link Set} of all parameter names in the collection
	 */
	public Set<String> getNames() {
		return params.keySet();
	}
	
	/**
	 * Returns the value of a parameter in the collection.
	 * If the parameter has multiple values, this returns a {@link List} of those values.
	 * 
	 * @param name	the name of a parameter
	 * @return		the value of the given parameter. If the parameter has multiple values, this returns a {@link List} of those values.
	 */
	public Object getValue(String name) {
		List<Object> value = params.get(name);
		if (value != null) {
			if (value.size() == 1) {
				return value.get(0);
			}
		}
		return value;
	}
	
	/**
	 * Returns all the parameters whose values are Strings.
	 * 
	 * @return	a List of {@link org.apache.http.NameValuePair} objects representing all the String parameters defined in the collection
	 */
	public List<NameValuePair> getStringParams() {
		ArrayList<NameValuePair> stringParams = new ArrayList<NameValuePair>();
        for (Map.Entry<String, List<Object>> param : params.entrySet()) {
            List<Object> value = param.getValue();
            for (Object o : value) {
            	if (o instanceof String) {
            		stringParams.add(new BasicNameValuePair(param.getKey(), o.toString()));
            	}
            }
        }
        return stringParams;
	}
	
	/**
	 * Returns all the parameters whose values are Files.
	 * 
	 * @return	a List of {@link java.util.Map.Entry} objects representing all the File parameters defined in the collection
	 */
	public List<Entry<String, File>> getFileParams() {
		ArrayList<Entry<String, File>> fileParams = new ArrayList<Entry<String, File>>();
        for (Map.Entry<String, List<Object>> param : params.entrySet()) {
            List<Object> value = param.getValue();
            for (Object o : value) {
            	if (o instanceof File) {
            		fileParams.add(new SimpleEntry<String, File>(param.getKey(), (File)o));
            	}
            }
        }
        return fileParams;
	}
	
	/**
	 * Returns whether the given parameter name has been assigned a value in this instance.
	 * 
	 * @param name	the name of a parameter
	 * @return		true if the given parameter name has been assigned a value
	 */
	public boolean isParameterSet(String name) {
		return params.containsKey(name);
	}
	
	private void addObject(String name, Object value) {
		if (params.containsKey(name)) {
			params.get(name).add(value);
		} else {
			ArrayList<Object> valueList = new ArrayList<Object>();
			valueList.add(value);
			params.put(name, valueList);
		}
	}
}
