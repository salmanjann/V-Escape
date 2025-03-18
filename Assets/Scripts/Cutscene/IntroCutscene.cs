using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroCutscene : ManagerCutscene
{
   protected override void initialize_Text()
   {
        Text.Add("In a world full of a unique and a roundabout species");
        Text.Add("Where every intellectually capable individual is a bean-ling instead of well ... hoomans");
        for(int i = 2; i < scenesCount; i++)
        {
            Text.Add("");
        }
   }
}
