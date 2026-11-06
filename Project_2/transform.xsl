<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
	<xsl:output method="html" indent="yes" encoding="utf-8"/>
	<xsl:template match="/">
		<html>
			<head>
				<title>Список наукових конференцій</title>
				<style>
					body { font-family: Arial, sans-serif; margin: 20px; }
					table { border-collapse: collapse; width: 100%; }
					th, td { border: 1px solid #ddd; padding: 8px; text-align: left; }
					th { background-color: #2b5797; color: white; }
					tr:nth-child(even) { background-color: #f2f2f2; }
				</style>
			</head>
			<body>
				<h2>Наукові конференції</h2>
				<table>
					<tr>
						<th>Назва</th>
						<th>Тематика</th>
						<th>Організатор</th>
						<th>Вартість</th>
						<th>Місце</th>
						<th>Терміни</th>
						<th>Дедлайн тез</th>
					</tr>
					<xsl:for-each select="Conferences/Conference">
						<tr>
							<td>
								<xsl:value-of select="@Title"/>
							</td>
							<td>
								<xsl:value-of select="@Field"/>
							</td>
							<td>
								<xsl:value-of select="@Organizer"/>
							</td>
							<td>
								<xsl:value-of select="@Fee"/>
							</td>
							<td>
								<xsl:value-of select="Details/@Location"/>
							</td>
							<td>
								<xsl:value-of select="Details/@StartDate"/>
							</td>
							<td>
								<xsl:value-of select="Details/@Deadline"/>
							</td>
						</tr>
					</xsl:for-each>
				</table>
			</body>
		</html>
	</xsl:template>
</xsl:stylesheet>