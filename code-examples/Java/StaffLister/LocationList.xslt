<?xml version="1.0"?>

<xsl:stylesheet version="1.0"
xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
xmlns:insites="http://www.intelligentinsites.net/api/rest">

<xsl:template match="/">
	<select id="locationselect">
		<xsl:for-each select="/insites:list-response/value">
			<xsl:element name="option">
				<xsl:attribute name="value">
					<xsl:value-of select="@id"/>
				</xsl:attribute>
				<xsl:value-of select="name"/>
			</xsl:element>
		</xsl:for-each>
	</select>
</xsl:template>

</xsl:stylesheet>