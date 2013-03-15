#This script computes the number of unique locations visited by each staff within the past day (since midnight).
#To execute this web service, send an HTTP GET request to InSites: /snap-in/insites/example/service/staff-daily-unique-locations.xml?staff=BxW,BxhK
#Use the 'staff' parameter to pass one or more staff IDs to the service.

require 'nokogiri'
require 'set'
require 'json'
java_import 'java.util.Date'
java_import 'java.text.SimpleDateFormat'

@log = @sandbox.log("wavemark-users")

response_format = "xml"

staff_select = "SELECT DISTINCT s.staff_id FROM role_snap_ins r INNER JOIN staff_role s ON r.role_id = s.role_id WHERE r.snap_in_id='insites-wavemark-page-wavemark'"
#staff and authorized functional roles
#select s.staff_id, rf.function_id from staff_role s inner join role_functions rf on s.role_id = rf.role_id

#staff that have access to wavemark snap-in, have logins, and have functional-roles with wavemark-department-id external-references. TODO: check if resource-state = active
#select s.staff_id, rf.function_id, dept.system_key, dept.external_key from staff_role s 
#    inner join role_functions rf 
#        on s.role_id = rf.role_id 
#    inner join function_external_reference dept
#        on dept.function_id = rf.function_id
#    inner join login l 
#        on l.staff_id = s.staff_id 
#    inner join role_snap_ins rs 
#        on s.role_id = rs.role_id 
#    where rs.snap_in_id = 'insites-wavemark-page-wavemark' and dept.system_key = 'wavemark-department-id'
    
login_functionalrole_query = "SELECT"
staff_ids = @internal_sandbox.query(staff_select)


#get movement history for the requested staff
history_xml = Nokogiri::XML(@sandbox.httpGet("/api/2.0/rest/history/movements/staff.xml", {'filter'=>filter, 'select'=>'location,staff'}))

#initialize history data structure. The Set provides uniqueness of location ids
history = Hash.new
staff_ids.each do |id|
    history[id] = Set.new
end

#populate the history Hash
history_xml.xpath('/insites:list-response/value').each do |node|
    history[node.xpath('./staff/@id').text].add(node.xpath('./location/@id').text)
end

#build a response with our results
case response_format
when "xml"
    builder = Nokogiri::XML::Builder.new do |xml|
        xml.response {
            xml.send(:'request-info') {
                @sandbox.request.parameters.keySet().each do |p|
                    xml.parameter(:param => p, :value => @sandbox.request.parameters[p])
                end
            }
            xml.date {
                xml.text midnight_time
            }
            xml.send(:'staff-list') {
                staff_ids.each do |id|
                    xml.staff(:id => id, :'unique-locations' => history[id].length)
                end
            }
        }
    end
    @sandbox.response.content_type = "text/xml"
    @sandbox.response.output = builder.to_xml
    
when "json"
    builder = Hash.new
    builder["request-info"] = Array.new
    @sandbox.request.parameters.keySet().each do |p|
        builder["request-info"] << Hash[:param => p, :value => @sandbox.request.parameters[p]]
    end
    builder["date"] = midnight_time
    builder["staff-list"] = Array.new
    staff_ids.each do |id|
        builder["staff-list"] << Hash[:id => id, :'unique-locations' => history[id].length]
    end
    @sandbox.response.content_type = "application/json"
    @sandbox.response.output = builder.to_json
else
    @sandbox.response.sendError(400)
end