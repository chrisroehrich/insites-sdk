require 'nokogiri'

@log = @sandbox.log('generate_tables')

def add_type_to_counts(total_counts, type_id, type_name, par_min, par_max)
  unless total_counts[type_id]
    total_counts[type_id] = {'name' => type_name, 'count' => 0, 'parMin' => par_min.eql?('') ? nil : par_min.to_i , 'parMax' => par_max.eql?('') ? nil : par_max.to_i}
  end
  total_counts[type_id]['count'] += 1
  total_counts
end


begin
  request_params = @sandbox.request.get_parameters

  #Get the location's name
  xml = Nokogiri::XML(@sandbox.httpGet("/api/2.0/rest/locations/#{request_params['location_id']}.xml", {'select' => 'name,location-types.id'}))
  location_name = xml.xpath('insites:location/name').text()

  #Find all par-levels associated with this location
  location_types = xml.xpath('insites:location/location-types/value/@id')
  filter_param = "applies-to-each-location eq 'true' and (location-type.id eq ''"
  location_types.each do |type|
    filter_param << " or location-type.id eq '#{type.text()}'"
  end
  filter_param << ')'
  par_levels_xml = Nokogiri::XML(@sandbox.httpGet('/api/internal/rest/par-levels.xml', {'filter' => filter_param, 'select' => 'entity-type.id, location-type.id, max, min', 'limit' => '-1'}))

  #Find the total count, and par level range of each equipment/supply type in this location
  total_counts = Hash.new
  #First, find entities in this location that match the search term
  entities_xml = Nokogiri::XML(@sandbox.httpGet('/api/2.0/rest/entities.xml', {'filter' => "(this matches '#{request_params['search']}' and current-location in '#{request_params['location_id']}') and (element-type eq 'equipment' or element-type eq 'supply')",
                                                                               'select' => 'type.id',
                                                                               'limit' => -1}))
  #Get a list of the entity-types corresponding to the matched entities
  entity_types = entities_xml.xpath('insites:list-response/value/type/@id')
  filter_param = "(current-location in '#{request_params['location_id']}') and (element-type eq 'equipment' or element-type eq 'supply') and (type.id eq ''"
  entity_types.each do |type|
    filter_param << " or type.id eq '#{type.text()}'"
  end
  filter_param << ')'
  #Find entities in this location corresponding to those entity-types
  entities_xml = Nokogiri::XML(@sandbox.httpGet('/api/2.0/rest/entities.xml', {'filter' => filter_param,
                                                                               'select' => 'type.id, type.name, type.parent-hierarchy.id, type.parent-hierarchy.name',
                                                                               'limit' => -1}))
  entity_location_types = entities_xml.xpath('insites:list-response/value/type')
  entity_location_types.each do |entity_type|
    entity_type_id = entity_type.xpath('@id').text()
    total_counts = add_type_to_counts(total_counts, entity_type_id, entity_type.xpath('name').text(), par_levels_xml.xpath("insites:list-response/value[entity-type/@id='#{entity_type_id}']/min").text(), par_levels_xml.xpath("insites:list-response/value[entity-type/@id='#{entity_type_id}']/max").text())
    #This entity-type might have ancestors. We want to count this entity as being one of those types too.
    ancestors = entity_type.xpath('parent-hierarchy/value')
    ancestors.each do |entity_type_ancestor|
      entity_type_id = entity_type_ancestor.xpath('@id').text()
      total_counts = add_type_to_counts(total_counts, entity_type_id, entity_type_ancestor.xpath('name').text(), par_levels_xml.xpath("insites:list-response/value[entity-type/@id='#{entity_type_id}']/min").text(), par_levels_xml.xpath("insites:list-response/value[entity-type/@id='#{entity_type_id}']/max").text())
    end
  end

  context = {'locationName' => location_name, 'entityTypes' => total_counts}

  @sandbox.response.content_type = 'text/html'
  @sandbox.response.output = @sandbox.template.render('par_level_table.vm', context)
rescue => e
  @sandbox.response.content_type = 'text/html'
  @sandbox.response.output =e.to_s
end