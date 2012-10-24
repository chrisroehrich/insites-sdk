package com.intelligentinsites.codesamples;

import android.os.AsyncTask;

/**
 * Executes HTTP requests as an AsyncTask.
 */
public class CallAPITask extends AsyncTask<String, Void, String> {
	
	@Override
	protected String doInBackground(String... params) {
    	BasicClient client = new BasicClient(SensorActivity.hostIP.getText().toString(),
    			SensorActivity.userName.getText().toString(),
    			SensorActivity.password.getText().toString(),
    			Integer.parseInt(SensorActivity.portNum.getText().toString()));
    	try {
    		return client.post(params[0]);
    	}
    	catch (Exception e)
    	{
    		return e.getMessage();
    	}
	}
	
	@Override
	protected void onPostExecute(String response) {
		System.out.println("Message Sent");
		System.out.println(response);
    }
}