using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Schema;

class XmlSchemaReadWriteExample
{
    public static string Run(string filename)
    {
        string output = "<default string>";
        try
        {
            StringWriter sw = new StringWriter();
            XmlTextReader reader = new XmlTextReader(filename);
            XmlSchema myschema = XmlSchema.Read(reader, ValidationCallback);
            myschema.Write(Console.Out);
            myschema.Write(sw);
            output = sw.ToString();
            FileStream file = new FileStream("new.xsd", FileMode.Create, FileAccess.ReadWrite);
            XmlTextWriter xwriter = new XmlTextWriter(file, new UTF8Encoding());
            xwriter.Formatting = Formatting.Indented;
            myschema.Write(xwriter);

            XmlSchemaSet schemaSet = new XmlSchemaSet();
            schemaSet.ValidationEventHandler += new ValidationEventHandler(ValidationCallbackOne);
            schemaSet.Add(myschema);
            schemaSet.Compile();

            XmlSchema compiledSchema = null;

            foreach (XmlSchema schema1 in schemaSet.Schemas())
            {
                compiledSchema = schema1;
            }

            XmlNamespaceManager nsmgr = new XmlNamespaceManager(new NameTable());
            nsmgr.AddNamespace("xs", "http://www.w3.org/2001/XMLSchema");
            compiledSchema.Write(Console.Out);
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
        return output;
    }

    static void ValidationCallback(object sender, ValidationEventArgs args)
    {
        if (args.Severity == XmlSeverityType.Warning)
            Console.Write("WARNING: ");
        else if (args.Severity == XmlSeverityType.Error)
            Console.Write("ERROR: ");

        Console.WriteLine(args.Message);
    }
    static void ValidationCallbackOne(object sender, ValidationEventArgs args)
    {
        Console.WriteLine(args.Message);
    }
}