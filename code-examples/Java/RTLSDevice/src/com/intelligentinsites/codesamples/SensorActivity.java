package com.intelligentinsites.codesamples;


import java.util.ArrayList;

import com.intelligentinsites.codesamples.R;

import android.os.Bundle;
import android.app.Activity;
import android.view.Menu;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;

/**
 * SensorActivity is the Activity which handles UI events.
 */
public class SensorActivity extends Activity {
	
	public static ArrayList<TextView> fields;
	public static EditText editLabel;
	public static EditText hostIP;
	public static EditText userName;
	public static EditText password;
	public static EditText portNum;
	
    @Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_sensor);
        
        fields = new ArrayList<TextView>();
        fields.add((Button) findViewById(R.id.button1));
        fields.add((Button) findViewById(R.id.button2));
        fields.add((Button) findViewById(R.id.button3));
        fields.add((Button) findViewById(R.id.button4));
        fields.add((Button) findViewById(R.id.buttonLowBatt));
        fields.add((Button) findViewById(R.id.buttonMove));
        fields.add((EditText) findViewById(R.id.editLocationId));
        
        editLabel = (EditText) findViewById(R.id.editLabel);
        editLabel.addTextChangedListener(new LabelTextWatcher());
        
        hostIP = ((EditText) findViewById(R.id.editHostIP));
        userName = ((EditText) findViewById(R.id.editUserName));
        password = ((EditText) findViewById(R.id.editPassword));
        portNum = ((EditText) findViewById(R.id.editPort));
    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        getMenuInflater().inflate(R.menu.activity_sensor, menu);
        return true;
    }
    
    public void btn1Press(View view) {
    	CallAPITask task = new CallAPITask();
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/button-press.xml?button=1", editLabel.getText()));
    }
    
    public void btn2Press(View view) {
    	CallAPITask task = new CallAPITask();
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/button-press.xml?button=2", editLabel.getText()));
    }
    
    public void btn3Press(View view) {
    	CallAPITask task = new CallAPITask();
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/button-press.xml?button=3", editLabel.getText()));
    }
    
    public void btn4Press(View view) {
    	CallAPITask task = new CallAPITask();
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/button-press.xml?button=4", editLabel.getText()));
    }
    
    public void btnLowBatteryPress(View view) {
    	CallAPITask task = new CallAPITask();
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/low-battery.xml", editLabel.getText()));
    }
    
    public void btnLocationChangePress(View view) {
    	CallAPITask task = new CallAPITask();
    	EditText editLocation = (EditText) findViewById(R.id.editLocationId);
    	task.execute(String.format("/api/2.0/rest/sensors/by-key/other-rtls/%s/move.xml?new-location=%s", editLabel.getText(), editLocation.getText()));
    }

    
}

