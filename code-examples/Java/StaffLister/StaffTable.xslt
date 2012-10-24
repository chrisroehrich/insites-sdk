<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:insites="http://www.intelligentinsites.net/api/rest">

<xsl:template match="/">
	<table id="staff">
		<xsl:choose>
			<xsl:when test="count(/insites:list-response/value) > 0">
				<thead>
					<th scope="col">Name</th>
					<th scope="col">Current Location</th>
					<th scope="col">Status</th>
					<th scope="col">Role</th>
				</thead>
				<tbody>
					<xsl:for-each select="/insites:list-response/value">
						<tr>
							<td><xsl:value-of select="name"/></td>
							<td><xsl:value-of select="current-location/name"/></td>
							<td><xsl:value-of select="status/name"/></td>
							<td><xsl:value-of select="type/name"/></td>
						</tr>
					</xsl:for-each>
				</tbody>
			</xsl:when>
			<xsl:otherwise>
				<tr>
					<td>There were no staff found in this location.</td>
				</tr>
			</xsl:otherwise>
		</xsl:choose>
	</table>
</xsl:template>

</xsl:stylesheet>