require 'nokogiri'
            
#Retrieve some data from InSites
equipment_xml = Nokogiri::XML(@sandbox.httpGet("/api/2.0/rest/equipment.xml", {"select"=>"name,status.name", "limit"=>"10"}))
            
#Parse the response data
equipment_list = [[]]
equipment_xml.xpath("/insites:list-response/value").each_with_index do |node, i|
    equipment_list[i] = [node.xpath("./name").text, node.xpath("./status/name").text]
end
            
#Add the value to a Hash
context = {"equipmentList" => equipment_list}
            
#Render the page, and put the resulting string into the HTTP response
@sandbox.response.content_type = "text/html"
@sandbox.response.output = @sandbox.template.render("statuses.vm", context)