using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class IntroCutscene : ManagerCutscene
{
   protected override void initialize_Text()
   {
        Text.Add("In a world full of a unique and a roundabout species");
        Text.Add("Where every intellectually capable individual is a bean-ling instead of well ... hoomans");
        Text.Add("Who's this fashionable fellow? ... that would be Lucian.. Lucian Grey a treasure hunter.");
        Text.Add("What would Lucian be doing in this bizzare world? ...");
        Text.Add("He is up looking for treasures to make a living and pay his expenses... he is usually broke so can't really say the job is doing him well by any means");
        Text.Add("Lucian Enters a bizzare town in search of a rumoured artifact worth Thousands of Beanoes on the Black market");
        Text.Add("As he enters the gloomy town he feels strange cold in his bones... its as if the colour of the life itself was taken away");
        Text.Add("Lucian finds an old bean at the cross road in the town... he confronts him trying to piece information as much as possible");
        Text.Add("The old bean tells Lucian about the horrors of the ruins that lie ahead.. seemingly normal but full of Devilish entities whose terror has been fortold in fairytales all over beankind");
        Text.Add("but along those fairy tales the old bean mentions a certain artifact one taht looks like it is made of jade but coloured unlike one as beautiful and the only thing full of colours in this forsaken land");
        Text.Add("Hearing about the object of his desire and his dinner being the fist thing on his mind lucian declares to the old bean how he is going to add dinner on his table tonight");
        Text.Add("the old bean warns him with great furiousness \" NO!!!! THOU WILL NOT MAKE IT EH!! I AM TELLING THY TO HALT WHERE THEE STAND, FOR IGNORING THIS CALL WILL PLACE WHAT COMES FORTH ON THY SOUL \". Lucian being him ignores warning afterall.. what can a man who is already on foot in the grave yet lives in this very town be onto something this serious... probably just doesnt want me to steal the valuables.. hah poor old stew");
        Text.Add("Lucian Barges into the most prominent looking house breaking the front door");
        Text.Add("It is a fairly spaced place but that won't stop this hunter to get the bread on his table");
        Text.Add("After hours of looking and searching every crevise of the house he finds a box hidden in one of the floor compartments");
        Text.Add("as he tries to dust it off the lid is opened and it flies back with such a shock wave that a grown bean flies back landing on his base");
        Text.Add("in a flurry he sees three distinct shapes the orb telling him that they are his trials... as the orb splits into 3 and merges into the shapes swiftly before vanishing completely");
        Text.Add("Lucian Feels uneasy and a sudden stroke to is heart... his body is failing him, the pain is so severe that his lungs feel filled yet gasping for air, his bones feel heavy, his muscles aching... there is a torch in his hand all of a sudden.. and as he turns it on the pain goes away... lucian knows what he must do... its no longer about dinner... its about surviving...");
   }
      protected override void OnSceneLoaded(Scene scene, LoadSceneMode mode)
      {
         if (scene.name == "Loading")
         {
            GameObject temp = GameObject.Find("EventSystemLoading");
            if (temp != null)
            {
                  LoadScreen loadScreen = temp.GetComponent<LoadScreen>();
                  loadScreen.previous = "Intro";
                  loadScreen.next = "PCG_House";
                  loadScreen.delay = 1;
            }
         }
         SceneManager.sceneLoaded -= OnSceneLoaded; // Unsubscribe after handling
      }
}
