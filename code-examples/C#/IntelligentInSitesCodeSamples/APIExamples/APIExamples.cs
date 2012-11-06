/*Intelligent InSites Web Service API Examples

This simple console application demonstrates many of the features of the
Intelligent InSites Web Service API. Simply un-comment each line to try them out.
This class Implements the BasicClient class to perform HTTP requests.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IntelligentInSites.CodeSamples {
    class APIExamples {
        static void Main(string[] args) {
            BasicClient client = new BasicClient("http://insites-dev.intelligentinsites.net", "username", "password");
            ApiResponse response;

            //// limit
            response = client.Get("/api/2.0/rest/equipment.xml", String.Empty);   //Get 100 equipment resources
            //response = client.Get("/api/2.0/rest/equipment.xml", "limit=3");      //Get 3 equipment resources
            //response = client.Get("/api/2.0/rest/equipment.xml", "limit=-1");     //Get all equipment resources

            //// limit + first-result
            //response = client.Get("/api/2.0/rest/equipment.xml", "limit=5&first-result=0");	//Get the first 5 equipment resources
            //response = client.Get("/api/2.0/rest/equipment.xml", "limit=5&first-result=5");	//Get the next 5 equipment resources

            //// select
            //response = client.Get("/api/2.0/rest/staff/BxhL.xml", String.Empty);								//Get the staff resource with an id of 'BxhL'
            //response = client.Get("/api/2.0/rest/staff/BxhL.xml", "select=name");					            //Get only the name field of the staff with an id of 'BxhL'
            //response = client.Get("/api/2.0/rest/staff/BxhL.xml", "select=name,current-location");	        //Get the name and location of the staff with an id of 'BxhL'

            //// expand
            //response = client.Get("/api/2.0/rest/logins.xml", "expand=staff"); 								//Get login resources with the 'staff' field expanded.
            //response = client.Get("/api/2.0/rest/logins.xml", "expand=staff.primary-location");				//Get logins with the associated staff's primary location expanded.
            //response = client.Get("/api/2.0/rest/equipment.xml", "expand=sensors,service-status");			//Get equipment with multiple fields expanded
            //response = client.Get("/api/2.0/rest/equipment.xml", "expand=sensors&expand=service-status");

            //// sort
            //response = client.Get("/api/2.0/rest/staff.xml", "sort=name");								//Sort all staff by name in ascending order, then get the first 100.
            //response = client.Get("/api/2.0/rest/staff.xml", "sort=name+asc");							//Sort all staff by name in ascending order, then get the first 100.
            //response = client.Get("/api/2.0/rest/staff.xml", "sort=name+desc");							//Sort all staff by name in descending order, then get the first 100.
            //response = client.Get("/api/2.0/rest/staff.xml", "sort=to-lower(name)+asc");				    //Sort all staff by name, ignoring case
            //response = client.Get("/api/2.0/rest/staff.xml", "sort=current-location.name&sort=name");	    //Sort all staff by location name, then by name

            //// filter
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=manufacturer~'Ekahau'");																		    //Get equipment where the manufacturer field is 'Ekahau'
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=manufacturer+eq+'Ekahau'");
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=manufacturer~'Ekahau'+and+current-location.name~'BioMed'");									    //Get equipment where manufacturer = 'Ekahau' and current location name = 'BioMed'
            //response = client.Get("/api/2.0/rest/patient-visits.xml", "filter=status.name+eq+'In+Prep'+and+(type.name+eq+'ER+Patient'+or+type.name+eq+'OR+Patient')");    //Get patient-visits with a status of 'In Prep', and whose type is either 'ER Patient', or 'OR Patient'
            //response = client.Get("/api/2.0/rest/entities.xml", "filter=sensors.total-count+gt+0");																		//Get entities with attached sensors
            //response = client.Get("/api/2.0/rest/sensors.xml", "filter=entity-attached-to.element-type+eq+'equipment'"); 												    //Get sensors attached to any equipment
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=this+matches+'pump'"); 																		    //Get equipment matching the search term 'pump'
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=name+matches+'IV.*'"); 																		    //Get equipment where 'name' matches the regex 'IV.*'
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=name+matches+'/iv.*/i'"); 																		//Case insensitive match by surrounding a regex with '/.../i'
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=current-location+in+'Bxdc'"); 																	//Get equipment within the location 'Bxdc' or any descendant locations within the location hierarchy
            //response = client.Get("/api/2.0/rest/locations.xml", "filter=this+in+'BxhM'"); 																				//Get the location 'BxhM' and its descendant locations

            //// field methods
            //response = client.Get("/api/2.0/rest/equipment.xml", "select=sensors.sort(provider+asc)");									//Sort each equipment's sensors by provider.
            //response = client.Get("/api/2.0/rest/equipment.xml", "select=sensors.get(0)");												//Get equipment and select only the first sensor from each.
            //response = client.Get("/api/2.0/rest/equipment.xml", "select=sensors.sort(provider+asc).get(0..2)");							//Sort each equipment's sensors by provider, then select the first 3 sensors from each.
            //response = client.Get("/api/2.0/rest/equipment.xml", "filter=sensors.filter(not-reporting+eq+'false').total-count+gt+0");		//Get equipment with at least one sensor that is not reporting
            //response = client.Get("/api/2.0/rest/locations.xml", "filter=parent-hierarchy.filter(name+eq+'Campus+1').total-count+gt+0");	//Get descendant locations of 'Campus 1'.

            //// Creating, Deleting, and Modifying Data
            //response = client.Post("/api/2.0/rest/equipment.xml", "name=Wheelchair+1&service-status=Bxc&short-name=wc1&status=Bxc&type=Bxc6x");	                                //Create a new equipment resource named 'Wheelchair 1'
            //response = client.Post("/api/2.0/rest/equipment/Bxj.xml", "model=X4000");															                                    //Update an existing equipment resource by changing the model to 'X4000'
            //response = client.Post("/api/2.0/rest/sensors/Bxrx/button-press.xml", "button=2");													                                //Inform InSites that button 2 was pressed on sensor 'Bxrx'
            //response = client.Post("/api/2.0/rest/sensors/by-key/ekahau-rtls/00:18:8e:20:44:a7/move.xml", "new-location=BxdL");	                                                //Inform InSites that the sensor with key/value (ekahau-rtls/00:18:8e:20:44:a7) has moved to location 'BxdL'
            //response = client.Post("/api/2.0/rest/locations/Bxc.xml", "attributes.insites.assigned-patient=Bxzkw");                                                               //Assign a value to the custom attribute assigned-patient
            //response = client.Post("/api/2.0/rest/locations/Bxc.xml", "attributes.insites.assigned-patient=");                                                                    //Clear the value of the custom attribute assigned-patient
            //response = client.Post("/api/2.0/rest/locations/BxjL.xml", "attributes.insites.required-equipment-types=Bxc7L&attributes.insites.required-equipment-types=Bxc7R");    //Assign multiple values to a custom attribute collection

            Console.WriteLine(response.ResponseData);
            Console.ReadLine();
        }
    }
}