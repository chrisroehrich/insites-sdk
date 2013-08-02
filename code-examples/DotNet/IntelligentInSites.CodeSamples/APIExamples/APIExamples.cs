/*Intelligent InSites Web Service API Examples

This simple console application demonstrates many of the features of the
Intelligent InSites Web Service API. Simply un-comment each line to try them out.
This class Implements the APIClient class to perform HTTP requests.*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IntelligentInSites.Api.Rest;
using System.IO;

namespace IntelligentInSites.CodeSamples {
    class APIExamples {
        static void Main(string[] args) {
            APIClient client = new APIClient(UriScheme.Https, "insites.dev.insitescloud.com", "username", "password", 443);

            APIResponse response;

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
            APIParams p = new APIParams();

            //Create a new equipment resource named 'Wheelchair 35'
            /*
            p.Add("name", "Wheelchair 35");
            p.Add("service-status", "Bxc");
            p.Add("short-name", "wc35");
            p.Add("status", "Bxc");
            p.Add("type", "Bxc6x");
            response = client.Post("/api/2.0/rest/equipment.xml", p);
            */

            //Update an existing equipment resource by changing the model to 'X4000'
            /*
            p.Add("model", "X4000");
            response = client.Post("/api/2.0/rest/equipment/Bxj.xml", p);
            */

            //Inform InSites that button 2 was pressed on sensor 'Bxrx'
            /*
            p.Add("button", 2);
            response = client.Post("/api/2.0/rest/sensors/Bxrx/button-press.xml", p);
            */

            //Inform InSites that the sensor with key/value (ekahau-rtls/00:18:8e:20:44:a7) has moved to location 'BxdL'
            /*
            p.Add("new-location", "BxdL");
            response = client.Post("/api/2.0/rest/sensors/by-key/ekahau-rtls/00:18:8e:20:44:a7/move.xml", p);
            */

            //Assign a value to the custom attribute assigned-patient
            /*
            p.Add("attributes.insites.assigned-patient", "Bxzkw");
            response = client.Post("/api/2.0/rest/locations/Bxc.xml", p);
            */

            //Clear the value of the custom attribute assigned-patient
            /*
            p.Add("attributes.insites.assigned-patient", String.Empty);
            response = client.Post("/api/2.0/rest/locations/Bxc.xml", p);
            */

            //Assign multiple values to a custom attribute collection
            /*
            p.Add("attributes.insites.assigned-patient", "Bxc7L");
            p.Add("attributes.insites.assigned-patient", "Bxc7R");
            response = client.Post("/api/2.0/rest/locations/BxjL.xml", p);
            */

            //Upload a file into a new binary-data resource.
    	    /*
            p.Add("original-file-name", "wheelchair.png");
    	    p.Add("data", File.ReadAllBytes(@"C:\data\wheelchair.png"));
    	    p.Add("mime-type", "image/png");
    	    response = client.PostMultipart("/api/2.0/rest/binary-data.xml", p);
            */

            Console.WriteLine(response.ResponseData);
            Console.ReadLine();
        }
    }
}