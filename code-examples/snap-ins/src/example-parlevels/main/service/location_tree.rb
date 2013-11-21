# Ruby script for handling the Location Ajax tree
require 'json'

@log = @sandbox.log('location-tree')

begin

request_params = @sandbox.request.get_parameters
response = []

if request_params['location_id']
  location_id = request_params['location_id'] if request_params['location_id']

  select_param = "id,name,has-children,location-level.rank"
  sort_param = 'name asc'
  limit_param = -1
  filter_param = "location-level.rank le 4 AND parent-location eq '%s'" % [location_id]

  json = JSON.parse(@sandbox.httpGet('/api/2.0/rest/locations.json', {'filter' => filter_param, 'select' => select_param, 'sort' => sort_param, 'limit' => limit_param}))

  if json['list-response']
    if json['list-response']['total-count'] != 0
      if not json['list-response']['value'].is_a? Array
        json['list-response']['value'] = [json['list-response']['value']]
      end
      json['list-response']['value'].each do |x|
        response.push({'id' => x['id'], 'title' => x['name'], 'state' => 'closed', 'hasChildren' => (x['has-children'] && x['location-level']['rank'] < 4)})
      end
    end
  end
end

@sandbox.response.content_type = 'application/json'
@sandbox.response.output = response.to_json
rescue => e
  
@sandbox.response.content_type = 'text/html'
@sandbox.response.output =e.to_s
  
end
