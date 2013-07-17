require 'nokogiri'

@log = @sandbox.log('index')

#We want each facility to have a different URL, so we are storing the URLs on a custom attribute of locations called url.
#To figure out which URL to redirect the current user to, find the value of url for the user's primary-location.
#If the user's primary-location does not have a value for url, look up the location tree until we find a location that does.
response_xml = Nokogiri::XML(@sandbox.httpGet("/api/2.0/rest/staff/by-request.xml?select=primary-location.attributes.filter(provider+eq+'example'+and+name+eq+'url').value.value,primary-location.parent-hierarchy.filter(attributes.filter(provider+eq+'example'+and+name+eq+'url').total-count+gt+0).sort(location-level.rank+desc).get(0..0).attributes.filter(provider+eq+'example'+and+name+eq+'url').value.value", Hash.new))
url = response_xml.xpath('./insites:staff/primary-location/attributes/value/value').text
if url == ''
  url = response_xml.xpath('./insites:staff/primary-location/parent-hierarchy/value/attributes/value/value').text
end

@sandbox.response.contentType='text/html'
if url == ''
  @sandbox.response.output =
      """
  <html>
    <head/>
    <body>
      No hostnames found for this facility. Please contact your System Administrator.
    </body>
  </html>
    """
else
  @sandbox.response.sendRedirect(url)
end