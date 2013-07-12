#This script computes the number of unique locations visited by each staff within the past day (since midnight).
#To execute this web service, send an HTTP GET request to InSites: /snap-in/insites/example/service/staff-daily-unique-locations.xml?staff=BxW,BxhK
#Use the 'staff' parameter to pass one or more staff IDs to the service.

require 'nokogiri'
require 'set'
require 'json'
require 'date'
require 'time'

@log = @sandbox.log("staff-daily-unique-locations")

response_format = @sandbox.request.url.split('.')[-1]

#get the time at midnight on today's date
now_time = Time.now
midnight_time = DateTime.new(now_time.year, now_time.month, now_time.day, 0, 0, 0, 0).strftime('%Y-%m-%dT%H:%M:%S%:z')

#parse the list of staff ids
staff_ids = @sandbox.request.parameters['staff'].split(',')

#build the filter expression for our history query
filter = "entered+gt+'#{midnight_time}'+and+("
staff_ids.each_with_index do |id, i|
    filter << "staff+eq+'#{id}'"
    if i < staff_ids.length - 1
        filter << "+or+"
    end
end
filter << ")"

@log.warn(filter)

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