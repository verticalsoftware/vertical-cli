namespace Vertical.Cli.Help;

public class XmlHelpProviderTests
{
    [Fact]
    public void Test()
    {
	    const string xml =
		    """
		    <?xml version="1.0" encoding="utf-8"?>
		    <help-content>
		    	<command id="root" name="root">
		    		<description>
		    			Some description of the root command.
		    		</description>
		    		<usage-grammar>
		    			<command-grammar>&lt;command&gt;</command-grammar>
		    			<arguments-grammar>arguments</arguments-grammar>
		    			<options-grammar>options</options-grammar>
		    		</usage-grammar>
		    		<symbols>
		    			<symbol id="root" grammar="ROOT" argument="PATH" sort-key="root">
		    				Description of the root command.
		    			</symbol>
		    		</symbols>
		    	</command>
		    </help-content>
		    """;

	    using var reader = new StringReader(xml);
    }
}