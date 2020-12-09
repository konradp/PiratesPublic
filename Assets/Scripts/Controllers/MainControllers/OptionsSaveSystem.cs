using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;

public class OptionsSaveSystem : MonoBehaviour
{
    private void Start()
    {
        Options curOpts;
        //check if file exists
        if (XMLManipulator.optionExists)
        {
            //if so, load them
            curOpts = XMLManipulator.LoadOptions();
            //if options haven't been assigned, reverse to default
            if (!curOpts.hasBeenAssigned)
                curOpts = DefaultOptions.GetDefaultOptions();

        }
        else
        {
            curOpts = DefaultOptions.GetDefaultOptions();
        }
        //let the class know we manipulated it
        curOpts.hasBeenAssigned = true;
        //we have loaded options, we need to feed them
        GlobalGameOptions.instance.SetGameOptions(curOpts);
        //save options just in case
        SaveOptions(curOpts);

    }
    void SaveOptions(Options _opts)
    {
        XMLManipulator.SaveOptions(_opts);
    }

}
