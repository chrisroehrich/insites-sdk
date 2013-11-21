require 'json'

context = Hash.new
context['request_url'] = @sandbox.request.url

select_param = "id,name"
sort_param = 'name asc'
limit_param = '-1'
filter_param="parent-location eq null AND location-level.rank le 3"

begin
  root_location_json = JSON.parse(@sandbox.httpGet("/api/2.0/rest/locations/enterprise.json",{'filter' => filter_param, 'select' => select_param, 'sort' => sort_param, 'limit' => limit_param}))
rescue => e
  context['error'] = e.to_s
end

context['enterprise_id'] = root_location_json['location']['id']
context['enterprise_name'] = root_location_json['location']['name']

@sandbox.response.content_type = 'text/html'
@sandbox.response.output = @sandbox.template.render('index.vm', context)