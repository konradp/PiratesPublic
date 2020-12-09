using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
public static class XMLManipulator
{
    public static void SaveOptions(Options _opts)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Options));
        StreamWriter writer = new StreamWriter(StaticOptionsVals.GetOptionsPath);
        serializer.Serialize(writer, _opts);
        writer.Close();
    }
    public static Options LoadOptions()
    {
        Options ret;
        XmlSerializer serializer = new XmlSerializer(typeof(Options));
        using (StreamReader reader = new StreamReader(StaticOptionsVals.GetOptionsPath))
        {
            ret = (Options)serializer.Deserialize(reader);
            reader.Close();
        }
        return ret;
    }
    public static bool optionExists => File.Exists(StaticOptionsVals.GetOptionsPath);
}
