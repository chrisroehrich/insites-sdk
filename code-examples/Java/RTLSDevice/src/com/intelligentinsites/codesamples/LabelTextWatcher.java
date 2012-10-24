package com.intelligentinsites.codesamples;

import android.text.Editable;
import android.text.TextWatcher;
import android.widget.TextView;

/**
 * LabelTextWatcher enables or disables UI fields depending on the value of the Label field.
 */
public class LabelTextWatcher implements TextWatcher {
    
    public LabelTextWatcher() {
    }

    public void beforeTextChanged(CharSequence s, int start, int count, int after) {
    }

    public void onTextChanged(CharSequence s, int start, int before, int count) {
    }

    /**
     * Enables fields if the value of the Label field is not empty, otherwise disables them.
     */
    public void afterTextChanged(Editable s) {
    	boolean enable = SensorActivity.editLabel.getText().length() > 0;
    	
    	if (!enable) {
    		for (TextView field : SensorActivity.fields) {
    			field.setEnabled(false);
    		}
    	}
    	else if (!SensorActivity.fields.get(0).isEnabled() && enable) {
    		for (TextView field : SensorActivity.fields) {
    			field.setEnabled(true);
    		}
    	}
    }
}