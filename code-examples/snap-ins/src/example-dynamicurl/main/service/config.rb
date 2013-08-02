#This script renders the URL config admin page
require 'json'

context = Hash.new
context['request_url'] = @sandbox.request.url
request_params = @sandbox.request.get_parameters

if request_params['location_id'] != nil
  begin
    @sandbox.httpPost("/api/2.0/rest/locations/#{request_params['location_id']}.json", {'attributes.example.url' => request_params['new_url']})
    context['success'] = 'Update Successful'
  rescue => e
    context['error'] = e.to_s
  end
end

select_param = "id,name,attributes.filter(provider+eq+'example'+and+name+eq+'url').value"
sort_param = 'name asc'
limit_param = '-1'
filter_param="parent-location eq null AND location-level.rank le 3"

begin
  root_location_json = JSON.parse(@sandbox.httpGet("/api/2.0/rest/locations/enterprise.json",{'filter' => filter_param, 'select' => select_param, 'sort' => sort_param, 'limit' => limit_param}))
rescue => e
  context['error'] = e.to_s
end

enterprise_id = root_location_json['location']['id']
enterprise_name = root_location_json['location']['name']
if root_location_json['location']['attributes']['value']
  enterprise_name = "#{enterprise_name}&nbsp;&nbsp;&nbsp;&nbsp;(#{root_location_json['location']['attributes']['value']['value']})"
end

context['enterprise_id'] = enterprise_id
context['enterprise_name'] = enterprise_name

@sandbox.response.content_type = 'text/html'
@sandbox.response.output = @sandbox.template.render('config.vm', context)