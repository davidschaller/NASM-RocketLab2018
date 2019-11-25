using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PopupHintGUI : MonoBehaviour
{
    public GUISkin hintSkin,
                   buttonSkin;

    GUIStyle contentStyle,      // default text
             insideBtnStyle,    // inside buttons
             headerStyle,       // headers
             smallStyle,        // hints
             smallHeaderStyle;  // hint headers 


    public Texture2D client1,
                     client2;

    bool activeHint = false;
    public bool Active
    {
        get
        {
            return activeHint;
        }
    }

    private ArchitectStudioGUI.States currentState;
    public bool IsShownAlready(ArchitectStudioGUI.States state)
    {
        return alreadyShown.Contains(state);
    }

    void Awake()
    {
        activeHint = false;

        contentStyle = hintSkin.GetStyle("window");
        insideBtnStyle = hintSkin.GetStyle("button");
        headerStyle = hintSkin.GetStyle("label");
        smallStyle = hintSkin.GetStyle("textarea");
        smallHeaderStyle = hintSkin.GetStyle("textfield");

        alreadyShown = new List<ArchitectStudioGUI.States>();

    }


    private List<ArchitectStudioGUI.States> alreadyShown;

    /// <summary>
    ///  Show hint by state
    /// </summary>
    /// <param name="state"></param>
    /// 
    public void ShowHint(ArchitectStudioGUI.States state)
    {
        currentState = state;
        activeHint = true;

        if (!alreadyShown.Contains(state))
            alreadyShown.Add(state);
    }


    #region  Clients Hint

    public Texture2D flwLarge;

    private enum clientsModes
    {
        main,
        clients,
        architects
    }
    private clientsModes clientsMode = clientsModes.main;

    private void ClientsHint()
    {
        switch (clientsMode)
        {
            case clientsModes.clients:
                ClientsHint_Clients();
                break;
            case clientsModes.architects:
                ClientsHint_Architects();
                break;
            default:
                ClientsHint_Main();
                break;
        }
    }

    private void ClientsHint_Main()
    {
        string header = "Wright's Hints To Making Clients Happy";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "When I design a house for a client, it helps me to " +
                      "know something about the people themselves. I " +
                      "consider their needs and what they like. I also think " +
                      "about how they will \"live\" in the home. I try to " +
                      "design it to make them happy and comfortable.";

        GUI.Label(new Rect(20, headerSize.y + 50, 400, 100), text, contentStyle);
        GUI.Label(new Rect(hintRect.width - flwLarge.width * 0.8f - 12,
                           hintRect.height - flwLarge.height * 0.8f - 13,
                           flwLarge.width * 0.8f,
                           flwLarge.height * 0.8f), flwLarge, GUIStyle.none);

        if (GUI.Button(new Rect(20, 25 + headerSize.y + 110, 100, 40), 
                "More about:\nCLIENTS", insideBtnStyle))
        {
            clientsMode = clientsModes.clients;
        }

        if (GUI.Button(new Rect(20, 25 + headerSize.y + 160, 100, 40), 
                "More about:\nARCHITECTS", insideBtnStyle))
        {
            clientsMode = clientsModes.architects;
        }
    }
    private void ClientsHint_Clients()
    {
        string header = "More About: Clients";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Clients hire an architect to define the kind of space they whould " +
                      "like built. Clients expect personalized designs based upon their " +
                      "needs, tastes, and budget.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 50), text, contentStyle);

        GUI.Label(new Rect(20, 25 + headerSize.y + 70, 170, 170), client1);
        GUI.Label(new Rect(20 + 70, 25 + headerSize.y + 70, 170, 170), client2);

        if (GUI.Button(new Rect(20, 25 + headerSize.y + 250, 100, 30), "GO BACK", insideBtnStyle))
        {
            clientsMode = clientsModes.main;
        }
    }
    private void ClientsHint_Architects()
    {
        string header = "More About: Architects";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Architects use skill, inspiration, and input from their cliants to " +
                      "crate built spaces. An architect uses knowledge of design, " +
                      "history, and structure to create buildings that function well for " +
                      "each client's needs.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 80), text, contentStyle);

        if (GUI.Button(new Rect(20, 25 + headerSize.y + 80, 100, 30), "GO BACK", insideBtnStyle))
        {
            clientsMode = clientsModes.main;
        }
    }

    #endregion

    #region Locations hint

    public Texture2D location;
    private bool moreAboutSites = false;

    private void LocationHint()
    {
        if (moreAboutSites)
            LocationHint_More();
        else
            LocationHint_Main();
    }

    private void LocationHint_More()
    {
        string header = "More About: Sites";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Every building is built on a site - it's the plot of land that the " +
                      "clients choose for their home. It can be anywhere: in a city, " +
                      "surburb, or remote area. It can be in the mountains, the desert," +
                      "or the seaside. Most architects think the relationship of the " +
                      "building to the site and enviropment is important. Do you?";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 130), text, contentStyle);

        GUI.Label(new Rect(20, headerSize.y + 130, 500, 20), "HINT:", smallHeaderStyle);

        string hint = "If the site has as particulary nice view of sunrises or sunsets, think about " +
                      "designing a building that showcases those views. Or if there are going to be " +
                      "neighbors next door, think about how to improve privacy and reduce noise";

        GUI.Label(new Rect(20, headerSize.y + 150, 500, 100), hint, smallStyle);

        if (GUI.Button(new Rect(20, headerSize.y + 220, 100, 25), "GO BACK", insideBtnStyle))
        {
            moreAboutSites = false;
        }
    }

    private void LocationHint_Main()
    {
        string header = "Wright's Hints About Choosing Locations";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "I always think about how the location will suit my clients. What will " +
                      "they see when they look out the window or step outside? For me, " +
                      "the climate and landscape affect the design of the house--which way " +
                      "it faces, the number of windows, the materials it’s made of. ";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        if (GUI.Button(new Rect(20, headerSize.y + 105, 100, 40), "More about:\nSITES", insideBtnStyle))
        {
            moreAboutSites = true;
        }

        GUI.Label(new Rect(hintRect.width / 2 - 263, hintRect.height - 250, 490, 225), location);
    }

    #endregion

    #region FloorPlans

    public Texture2D floorPlan;
    public Texture2D archiSymbols,
                     typesOfDrawnings,
                     sizes;

    private enum floorPlanModes
    {
        main,
        symbols,
        drawnings,
        proportions,
        scales,
        sizes
    }
    private floorPlanModes floorPlanMode = floorPlanModes.main;
    private void FloorPlanHint()
    {
        switch (floorPlanMode)
        {
            case floorPlanModes.drawnings:
                FloorPlanHint_Drawnings();
                break;
            case floorPlanModes.proportions:
                FloorPlanHint_Proportions();
                break;
            case floorPlanModes.scales:
                FloorPlanHint_Scales();
                break;
            case floorPlanModes.sizes:
                FloorPlanHint_Sises();
                break;
            case floorPlanModes.symbols:
                FloorPlanHint_Symbols();
                break;
            default:
                FloorPlanHint_Main();
                break;
        }
    }

    private void FloorPlanHint_Symbols()
    {
        string header = "More About: Architectural Symbols";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "How do the people who construct buildings know how to read the plans? " +
                      "All architects use a standard set of symbols on their floorplans and elevations " +
                      "so that builders will understand how to read them. Here are some examples:";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, 120, 500, 340), archiSymbols, GUIStyle.none);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.main;
        }
    }

    private void FloorPlanHint_Drawnings()
    {
        string header = "More About: Architectural Symbols";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Architects use different types of drawnings tp lay out the design of a " +
                      "building and describe how it will look when it's done. It helps them" +
                      "experiment with the best use of space for the site. It also gives their" +
                      "clients an idea of what they will be getting";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, 120, 540, 260),typesOfDrawnings, GUIStyle.none);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.main;
        }
    }

    private void FloorPlanHint_Proportions()
    {
        string header = "More About: Proportions";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Proportion is the relationship between your size and the size of" +
                      "the space you are in. Architects understand that people feel " +
                      "differently in different sized spaces. They tend to fill smaller in " +
                      "large spaces and larger in small spaces. But making a space too " +
                      "large can make you fell lost, and making it too small can make " + 
                      "you feel constricted. On this Website, each square on the" + 
                      "floorplan reprecents 3' x 3' of space. Three feet is the typical " + 
                      "width of doors and hallways. Keep this in mind while designing " + 
                      "rooms for your clients.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 200), text, contentStyle);

        GUI.Label(new Rect(20, headerSize.y + 200, 500, 20), "HINT:", smallHeaderStyle);

        string hint = "Bedrooms are usually bigger than bathrooms abd closets. Living and dining " +
                      "room are usually bigger than bedrooms. How will your clients use the space in" +
                      "their house, and how much space will they need for different activities? What "+
                      "size rooms will your clients need?";

        GUI.Label(new Rect(20, headerSize.y + 220, 500, 100), hint, smallStyle);

        if (GUI.Button(new Rect(20, 320, 100, 25), "GO BACK", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.main;
        }
    }

    private void FloorPlanHint_Scales()
    {
        string header = "More About: Scales";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Because you cannot draw draw the actual size of hause you must " +
                      "draw it to a smaller scale. This means assigning a smaller" + 
                      "measurement to represent a larger measurement. In the US, a" + 
                      "standard scale id 1 foot represents 1/4 inch. This way, you can fit" +
                      "event the largest building on a piece of paper. Different countries " +
                      "have different units of measurement. In America, we use feet " +
                      "and inches, and most everywhere else in the world they are meters " +
                      "and centimeters. The way to write on foot and two inches on paper is: " +
                      "1' - 2''. The way to write on meter and two centimeters is: 1.02m. " +
                      "On this Web site, each square in your floorplan reprecents 3 feet by 3 feet.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 200), text, contentStyle);

        GUI.Label(new Rect(20, headerSize.y + 200, 500, 20), "HINT:", smallHeaderStyle);

        string hint = "One meter approximately the same as three feet. On this Web site we show " +
                      "both types of measurement because architects have to work with people from " +
                      "all over the world and have to be able to use both feet and meters.";

        GUI.Label(new Rect(20, headerSize.y + 220, 500, 100), hint, smallStyle);

        if (GUI.Button(new Rect(20, 320, 100, 25), "GO BACK", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.main;
        }
    }

    private void FloorPlanHint_Sises()
    {
        string header = "More About: Standard Sizes";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Architects have found that certain sizes work well for rooms" +
                      "with specific uses. There is a standard width for doors and " +
                      "hallways, and a standard height for walls and ceilings. This does" +
                      "not always work for very short people, however, so " + 
                      "keep your client's specific needs in mind.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, headerSize.y + 120, 500, 20), "HINT:", smallHeaderStyle);
        string hint = "Here are some examples of standard sizes for rooms that most people will feel" +
                      "comfortable in.";

        GUI.Label(new Rect(20, headerSize.y + 140, 500, 100), hint, smallStyle);

        GUI.Label(new Rect(40, 220, 460, 140), sizes, GUIStyle.none);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.main;
            roomMode = roomsModes.main;
        }
    }

    private void FloorPlanHint_Main()
    {
        string header = "Wright's Hints About Drawing Floor Plans";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text1 = "I think the floor plan is very important when designing a house. " +
                       "A floor plan is a diagram of the house that shows the layout of rooms. " +
                       "Most floor plans are an arrangement of different shapes.";
        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text1, contentStyle);

        string text2 = "Can you visualize how your floorplan will fit on your site? Which side of your " + 
                       "building will be the front? Dow you want it to be obvious or more mysterious? " + 
                       "Have you chosen a floorplan enough square footage to fit the size of the family " + 
                       "who will live there, but not so much that they will lost in it?";
        GUI.Label(new Rect(20, headerSize.y + 110, 500, 120), text2, contentStyle);

        GUI.Label(new Rect(hintRect.width - 440, hintRect.height - 165, 385, 145), floorPlan);

        if (GUI.Button(new Rect(20, 200, 120, 50),
                "MORE ABOUT:\nARCHITECTURAL\nSYMBOLS", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.symbols;
        }

        if (GUI.Button(new Rect(20, 255, 100, 40),
                "MORE ABOUT:\nPROPORTIONS", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.proportions;
        }

        if (GUI.Button(new Rect(20, 305, 100, 40),
                "MORE ABOUT:\nSCALES", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.scales;
        }

        if (GUI.Button(new Rect(20, 355, 100, 50),
                "MORE ABOUT:\nSTANDART\nSIZES", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.sizes;
        }

        if (GUI.Button(new Rect(150, 200, 100, 50),
                "MORE ABOUT:\nTYPES OF\nDRAWNINGS", insideBtnStyle))
        {
            floorPlanMode = floorPlanModes.drawnings;
        }
    }

    #endregion

    #region House Heights Hint

    public Texture2D height;

    private void HouseHeightsHint()
    {
        string header = "Wright's Hints About House Heights";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "When I designing rooms, I " +
                      "visualize my clients walking " +
                      "through the house. How " +
                      "comfortable are they moving " +
                      "through the doorways and " +
                      "standing in the rooms? Are the " +
                      "spaces proportional to their " +
                      "sizes? Remember - children " +
                      "grow and families change over time.";

        GUI.Label(new Rect(20, headerSize.y + 50, 250, 200), text, contentStyle);

        GUI.Label(new Rect(270, 80, 240, 300), height, GUIStyle.none);  
    }

    #endregion

    #region Materials Hint

    private bool moreAboutMaterials = false;
    public Texture2D materials,
                     moreMaterials;

    private void MaterialsHint()
    {
        if (moreAboutMaterials)
            MaterialsHint_More();
        else
            MaterialsHint_Main();
    }

    private void MaterialsHint_Main()
    {
        string header = "Wright's Hints About Exterior Materials";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "When choosing materials, I think about my client and their site. I " +
                      "look  at the colors and textures of the materials and decide whether I " +
                      "want my building to blend into the site or to contrast with it. What " +
                      "kinds of materials will appeal to your clients?";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        if (GUI.Button(new Rect(20, 125, 120, 50),
                "More about:\nEXTERIOR MATERIALS", insideBtnStyle))
        {
            moreAboutMaterials = true;
        }

        GUI.Label(new Rect(90, 180, 381, 207), materials, GUIStyle.none);           
    }

    private void MaterialsHint_More()
    {
        string header = "More About: Exterior Materials";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "More choosing an exterior material, think about how it will " +
                      "make the house look on the site - do you wan to blend in or " +
                      "stand out? Do you want it to look traditional or modern? Also " +
                      "remember where your house is located, and chat different " +
                      "materials work well in different environments.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, headerSize.y + 120, 500, 20), "HINT:", smallHeaderStyle);
        string hint = "Stone keeps out the cold and holds up well against weather and age. Wood is " +
                      "easy to build with and can be painted or stained different colors. Glass does not " +
                      "have good insulation or privacy but makes ";

        GUI.Label(new Rect(20, headerSize.y + 140, 500, 100), hint, smallStyle);

        GUI.Label(new Rect(50, 220, 442, 145), moreMaterials, GUIStyle.none);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            moreAboutMaterials = false;
        }         
    }

    #endregion

    #region Roof Hint

    private bool moreRoofInfo = false;
    public Texture2D roofs,
                     moreRoofs;

    private void RoofHint()
    {
        if (moreRoofInfo)
            RoofHint_More();
        else
            RoofHint_Main();
    }

    private void RoofHint_Main()
    {
        string header = "Wright's Hints About Designing Roofs";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "When choosing a roof, I imagine the silhouette of the roufline against " +
                      "the sky. Different roof styles can make a house appear simpler or " +
                      "more complex. Which do you think your clients will like? Is the height " +
                      "of your roof in proportion to the height of the walls? Will the roof " + 
                      "material and shape be able to withstand the weather at your site?";


        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        if (GUI.Button(new Rect(30, 140, 120, 40),
                "More about: ROOFS", insideBtnStyle))
        {
            moreRoofInfo = true;
        }

        GUI.Label(new Rect(30, 180, 485, 210), roofs, GUIStyle.none);            
    }

    private void RoofHint_More()
    {
        string header = "More About: Roofs";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "A roof covers a building and protects the people inside from " +
                      "weather. The shape and material of a roof often depends on its " +
                      "environment.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(200, 100, 200, 20), "HINT:", smallHeaderStyle);
        string hint = "Houses that experience snow and rain often " +
                      "have sharply angled roofs so that the water will" +
                      "fall off. Houses in the desert usually have flat " +
                      "roofs because they stay cooler in the hot " +
                      "weather. Roofs can ba made out of shingles, " +
                      "metal, thatch, tile, turf, wood, and many other " +
                      "materials.";

        GUI.Label(new Rect(200, 130, 300, 200), hint, smallStyle);

        GUI.Label(new Rect(20, 100, 170, 190), moreRoofs, GUIStyle.none);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            moreRoofInfo = false;
        }          
    }

    #endregion

    #region Room Hint

    public Texture2D rooms;

    private enum roomsModes
    {
        main,
        spaces,
        sizes
    }
    private roomsModes roomMode = roomsModes.main;

    private void RoomsHint()
    {
        switch (roomMode)
        {
            case roomsModes.sizes:
                FloorPlanHint_Sises();
                break;
            case roomsModes.spaces:
                RoomsHint_Spaces();
                break;
            default:
                RoomsHint_Main();
                break;
        }       
    }

    private void RoomsHint_Main()
    {
        string header = "Wright's Hints About Rooms";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "I always think about how my clients will use each room. " + 
                      "Which room might be the one where people gather the most? What " + 
                      "might they be doing right before or after they use each space? " + 
                      "Will they be able to move easily from one activity to another? " + 
                      "Does the size of each room match its function? Are the private " + 
                      "spaces really private?";

        GUI.Label(new Rect(20, headerSize.y + 50, 400, 200), text, contentStyle);

        if (GUI.Button(new Rect(20, 160, 120, 40),
                "More about:\nINTERIOR SPACES", insideBtnStyle))
        {
            roomMode = roomsModes.spaces;
        }

        if (GUI.Button(new Rect(20, 210, 120, 40),
                "More about:\nSTANDART SIZES", insideBtnStyle))
        {
            roomMode = roomsModes.sizes;
        }

        GUI.Label(new Rect(hintRect.width - 450, hintRect.height - 255, 385, 215), rooms);
    }

    private void RoomsHint_Spaces()
    {
        string header = "More About: Interior Spaces";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Think about how each space in your house will be used: is it for " + 
                      "work, relaxation, entertainment, or just getting from one room " +
                      "to another? Different spaces can make people feel powerful, " +
                      "safe, playful, or relaxed. What will your clients do in the space, " +
                      "and how can you design the space to improve their experience?";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, 150, 500, 20), "HINT:", smallHeaderStyle);
        string hint = "Stone keeps out the cold and holds up well against weather and age. " +
                      "Wood is easy to building with and can be painted or stained different colors. " +
                      "Glass doesn not have good isulation or privacy but makes a building feel very open " +
                      "and lets in a lot of natural light.";

        GUI.Label(new Rect(20, 170, 500, 100), hint, smallStyle);

        if (GUI.Button(new Rect(20, 250, 100, 25), "GO BACK", insideBtnStyle))
        {
            roomMode = roomsModes.main;
        }          
    }

    #endregion

    #region Wall Hint

    public Texture2D walls;
    public Texture2D wallMore;

    private bool moreWallsInfo = false;

    private void WallsHint()
    {
        if (moreWallsInfo)
            WallsHint_More();
        else
            WallsHint_Main();
    }

    private void WallsHint_More()
    {
        string header = "More About: Walls";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Walls define space. They create room, support ceilings, and shape the " + 
                      "spaces that people use every day. Walls can be straight or curved, thick " + 
                      "or thin, tall or short. They can be made out of practically any material, " + 
                      "including wood, paper, glass, and stone. They can be made to stay in place or to move.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 130), text, contentStyle);

        GUI.Label(new Rect(20, 150, 294, 147), wallMore);

        if (GUI.Button(new Rect(20, headerSize.y + 300, 100, 25), "GO BACK", insideBtnStyle))
        {
            moreWallsInfo = false;
        }         
    }

    private void WallsHint_Main()
    {
        string header = "Wright's Hints About Walls";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "I know that the walls I build will shape my client's lifestyle, " + 
                      "so I think about my client's interests and hobbies. Do they like " + 
                      "to be around people and want large social areas? Or would they prefer " + 
                      "a quiet and peaceful atmosphere with smaller spaces? Will their home be " + 
                      "a social center or a retreat from the world? Don't forget closets!";

        GUI.Label(new Rect(20, headerSize.y + 50, 250, 200), text, contentStyle);

        if (GUI.Button(new Rect(20, 230, 100, 40), "More about:\nWALLS", insideBtnStyle))
        {
            moreWallsInfo = true;
        }

        GUI.Label(new Rect(hintRect.width - 320, hintRect.height - 330, 245, 285), walls);
    }

    #endregion

    #region Openings Hint

    public Texture2D openings,
                     doorsMore,
                     windowsMore;

    private enum openingsModes
    {
        main,
        doors,
        windows
    }
    private openingsModes openingsMode = openingsModes.main;

    private void OpeningsHint()
    {
        switch (openingsMode)
        {
            case openingsModes.doors:
                OpeningsHint_Doors();
                break;
            case openingsModes.windows:
                OpeningsHint_Windows();
                break;
            default:
                OpeningsHint_Main();
                break;
        }
    }

    private void OpeningsHint_Windows()
    {
        string header = "More About: Doors";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "A window is an opening in a wall that  allows lignt and air into a " +
                      "building. Windows are generally made of glass and can be any " +
                      "size. Many people appreciate natural light in their home and " +
                      "prefer having numerous windows.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, 100, 293, 189), windowsMore, GUIStyle.none);

        GUI.Label(new Rect(320, headerSize.y + 90, 500, 20), "HINT:", smallHeaderStyle);
        string hint = "Will your clients want higher " +
                      "windows for more privacy? " +
                      "Will they want lots of " +
                      "Will they want west windows " +
                      "to watch the sunset?";

        GUI.Label(new Rect(320, headerSize.y + 110, 200, 300), hint, smallStyle);

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            openingsMode = openingsModes.main;
        }          
    }

    private void OpeningsHint_Doors()
    {
        string header = "More About: Doors";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Doorsways provide access to different areas of a house. Doors " +
                      "control access while providing privacy. They separate rooms " +
                      "from view and noise.";

        GUI.Label(new Rect(20, headerSize.y + 50, 500, 100), text, contentStyle);

        GUI.Label(new Rect(20, 100, 261, 166), doorsMore, GUIStyle.none);

        GUI.Label(new Rect(300, headerSize.y + 70, 500, 20), "HINT:", smallHeaderStyle);
        string hint = "Doors can plain " + 
                      "or fancy. They can swing on " + 
                      "hinges, slide sideways, or " + 
                      "even be raised vertically. " +
                      "A heavy door can be used " +
                      "for protection or to block " +
                      "out noise from a room, " +
                      "while a lightweight door " +
                      "can be used for privacy. " +
                      "Glass doors can shut out " +
                      "noise while allowing a " +
                      "view into an area.";

        GUI.Label(new Rect(300, headerSize.y + 90, 200, 300), hint, smallStyle);

        

        if (GUI.Button(new Rect(20, 375, 100, 25), "GO BACK", insideBtnStyle))
        {
            openingsMode = openingsModes.main;
        }        
    }

    private void OpeningsHint_Main()
    {
        string header = "Wright's Hints About Openings";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "I know that the walls I build will shape my clients' lifestyle, " + 
                      "so I think about my clients' interests and hobbies. Do they like to be around " + 
                      "people and want large social areas? Or would they prefer a quiet and peaceful " + 
                      "atmosphere with smaller spaces? Will their home be a social center or a retreat " +
                      "from the world? Don't forget closets!";

        GUI.Label(new Rect(20, headerSize.y + 50, 250, 300), text, contentStyle);

        string text2 = "Windows placement can serve many purposes. I think about which direction each " + 
                       "room is facing. Do the clients want lots of bright light, or more indirect light? " + 
                       "Is the space private? Does it have a great view? Does it look welcoming from outside?";
        GUI.Label(new Rect(20, headerSize.y + 250, 250, 300), text2, contentStyle);

        GUI.Label(new Rect(hintRect.width - 330, hintRect.height - 360, 245, 285), openings);

        if (GUI.Button(new Rect(270, 350, 100, 40), "More about:\nDOORS", insideBtnStyle))
            openingsMode = openingsModes.doors;

        if (GUI.Button(new Rect(380, 350, 100, 40), "More about:\nWINDOWS", insideBtnStyle))
            openingsMode = openingsModes.windows;
    }

    #endregion

    #region Furnishings Hint

    private bool moreAboutInteriorDesigns = false;
    public Texture2D furnishings,
                     furnishingsMore;

    private void FurnishingsHint()
    {
        if (moreAboutInteriorDesigns)
            FurnishingsHint_More();
        else
            FurnishingsHint_Main();
    }

    private void FurnishingsHint_More()
    {
        string header = "More About: Interior Designs";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "The Interior of a house is much more than an arrangement of " +
                      "rooms and space. The design of a room appeals to many sences." +
                      "Think about what your clients will see, touch, and experience in " +
                      "their house.";
                        
        GUI.Label(new Rect(20, headerSize.y + 50, 500, 200), text, contentStyle);

        GUI.Label(new Rect(20, 110, 500, 20), "HINT:", smallHeaderStyle);

        string hint = "Blue is a claiming color while red is fiery and passionate. Glass and metal lend a " + 
                      "modern and industrial feel while wood and tile tend to be more traditional. Here are " +
                      "some things to keep in mind while designing your interiors:\n" + 
                      "\n" +
                      "* Color, pattern, lighting (what you see = visual)\n" +
                      "* Surface, texture, shape (what you toush = tactile)\n" +
                      "* Rhythm, balance, mass (what you experiaence = spatial)";

        GUI.Label(new Rect(20, 130, 500, 100), hint, smallStyle);

        GUI.Label(new Rect(20, 190, 510, 184), furnishingsMore, GUIStyle.none);

        if (GUI.Button(new Rect(20, 380, 100, 25), "GO BACK", insideBtnStyle))
        {
            moreAboutInteriorDesigns = false;
        }
                     
    }

    private void FurnishingsHint_Main()
    {
        string header = "Wright's Hints Furnishings";
        Vector2 headerSize = headerStyle.CalcSize(new GUIContent(header));
        GUI.Label(new Rect(20, 25, headerSize.x, headerSize.y), header, headerStyle);

        string text = "Before furnishing a house, I always double-check my clients' list of needs. " + 
                      "I think about the use of each room. Do the furnishings match the style of " + 
                      "the home and the lifestyle of the clients? Does the furniture seem too big " + 
                      "or too small for the room? Will the clients be comfortable in it?";

        GUI.Label(new Rect(20, headerSize.y + 50, 250, 300), text, contentStyle);

        if (GUI.Button(new Rect(20, 200, 120, 50), "More about:\nINTERIOR DESIGNS", insideBtnStyle))
        {
            moreAboutInteriorDesigns = true;
        }

        GUI.Label(new Rect(280, 100, 245, 285), furnishings);
    }

    #endregion


    static Rect hintRect;
    public float hintWidth = 600,
                 hintHeight = 420;

    private void BaseHint()
    {
        GUI.Box(hintRect, GUIContent.none);

        GUIStyle buttonStyle = buttonSkin.GetStyle("button");
        string buttonText = "CLOSE";
        Vector2 buttonSize = buttonStyle.CalcSize(new GUIContent(buttonText));

        if (GUI.Button(new Rect(hintRect.x + hintRect.width - buttonSize.x - 80,
                                  hintRect.y + 25,
                                  buttonSize.x,
                                  buttonSize.y), buttonText, buttonStyle))
        {
            HideHints();
        }

        buttonText = "CONTINUE";
        buttonSize = buttonStyle.CalcSize(new GUIContent(buttonText));

        if (GUI.Button(new Rect(hintRect.x + hintRect.width / 2 - buttonSize.x / 2,
                                  hintRect.y + hintRect.height - buttonSize.y,
                                  buttonSize.x,
                                  buttonSize.y), buttonText))
        {
            HideHints();
        }

    }

    private void RestoreAllHintStates()
    {
        clientsMode = clientsModes.main;
        moreAboutSites = false;
        floorPlanMode = floorPlanModes.main;
        moreAboutMaterials = false;
        moreRoofInfo = false;
        roomMode = roomsModes.main;
        moreWallsInfo = false;
        openingsMode = openingsModes.main;
        moreAboutInteriorDesigns = false;
    }

    internal string GetHintNameByState(ArchitectStudioGUI.States currentState)
    {
        switch (currentState)
        {
            case ArchitectStudioGUI.States.DesignExterior:
                switch (ExteriorDesignGUI.TabStage)
                {
                    case ExteriorDesignGUI.TabStages.FloorPlan:
                        return "FLOOR PLAN";
                    case ExteriorDesignGUI.TabStages.HouseHeight:
                        return "HEIGHT";
                    case ExteriorDesignGUI.TabStages.ExteriorMaterials:
                        return "MATERIALS";
                    case ExteriorDesignGUI.TabStages.Roof:
                        return "ROOF";
                    default:
                        return string.Empty;
                }
            case ArchitectStudioGUI.States.Layout:
                return "LOCATION";
            case ArchitectStudioGUI.States.ChooseClient:
                return "CLIENTS";
            case ArchitectStudioGUI.States.Interior:
                switch (InteriorDesignGUI.CurrentTab)
                {
                    case InteriorDesignGUI.Tabs.Rooms:
                        return "ROOMS";
                    case InteriorDesignGUI.Tabs.Walls:
                        return "WALLS";
                    case InteriorDesignGUI.Tabs.Openings:
                        return "OPENINGS";
                    case InteriorDesignGUI.Tabs.Furnishings:
                        return "FURNISHINGS";
                    default:
                        return string.Empty;
                }
                
            default:
                return string.Empty;
        }
    }

    internal void HideHints()
    {
        RestoreAllHintStates();
        activeHint = false;
    }

    static float delaySlideTime = 1f;
    static float currentSlideTime = 0f;

    static Vector3 tOffset = new Vector3(10.0f, Screen.height - 100, 0.0f); // WRIGHT'S HINTS button choords
    static Quaternion tRotation = Quaternion.Euler(0, 0, 0);
    static Vector3 tScale = new Vector3(0.1f, 0.1f, 0.1f);

    private void DrawHintsWindow(ArchitectStudioGUI.States state, float delayTime)
    {
        if (delayTime == 0)
            delayTime = 1;

        if (activeHint && tScale != Vector3.one)
        {
            tOffset = Vector3.Lerp(new Vector3(10.0f, Screen.height - 100),
                new Vector3(Screen.width / 2 - hintWidth / 2, Screen.height / 2 - hintHeight / 2), 
                    currentSlideTime / delayTime);

            tScale = Vector3.Lerp(new Vector3(0.1f, 0.1f, 0.1f), Vector3.one, currentSlideTime / delayTime);
            Matrix4x4 tMatrix = Matrix4x4.TRS(tOffset, tRotation, tScale);
            GUI.matrix = tMatrix;

            if (tScale == Vector3.one)
                currentSlideTime = 0;
            else
                currentSlideTime += Time.deltaTime;

            hintRect = new Rect(0, 0, hintWidth, hintHeight);

        }
        else if (!activeHint && tScale != new Vector3(0.1f, 0.1f, 0.1f))
        {
            tOffset = Vector3.Lerp(new Vector3(Screen.width / 2 - hintWidth / 2, Screen.height / 2 - hintHeight / 2),
                new Vector3(10.0f, Screen.height - 100),
                    currentSlideTime / delayTime);

            tScale = Vector3.Lerp(Vector3.one, new Vector3(0.1f, 0.1f, 0.1f), currentSlideTime / delayTime);
            Matrix4x4 tMatrix = Matrix4x4.TRS(tOffset, tRotation, tScale);
            GUI.matrix = tMatrix;
            currentSlideTime += Time.deltaTime;

            if (tScale == new Vector3(0.1f, 0.1f, 0.1f))
                currentSlideTime = 0;
            else
                currentSlideTime += Time.deltaTime;

            hintRect = new Rect(0, 0, hintWidth, hintHeight);
        }
        else
        {
            hintRect = new Rect(Screen.width / 2 - hintWidth / 2, Screen.height / 2 - hintHeight / 2, hintWidth, hintHeight);
        }

        BaseHint();
        GUILayout.BeginArea(hintRect);
        switch (state)
        {
            case ArchitectStudioGUI.States.ChooseClient:
                ClientsHint();
                break;
            case ArchitectStudioGUI.States.DesignExterior:

                switch (ExteriorDesignGUI.TabStage)
                {
                    case ExteriorDesignGUI.TabStages.FloorPlan:
                        FloorPlanHint();
                        break;
                    case ExteriorDesignGUI.TabStages.HouseHeight:
                        HouseHeightsHint();
                        break;
                    case ExteriorDesignGUI.TabStages.ExteriorMaterials:
                        MaterialsHint();
                        break;
                    case ExteriorDesignGUI.TabStages.Roof:
                        RoofHint();
                        break;
                    default:
                        FloorPlanHint();
                        break;
                }

                
                break;
            case ArchitectStudioGUI.States.Layout:
                LocationHint();
                break;
            case ArchitectStudioGUI.States.Interior:

                switch (InteriorDesignGUI.CurrentTab)
                {
                    case InteriorDesignGUI.Tabs.Rooms:
                        RoomsHint();
                        break;
                    case InteriorDesignGUI.Tabs.Walls:
                        WallsHint();
                        break;
                    case InteriorDesignGUI.Tabs.Openings:
                        OpeningsHint();
                        break;
                    case InteriorDesignGUI.Tabs.Furnishings:
                        FurnishingsHint();
                        break;
                }
                break;
            default:
                ClientsHint();
                break;
        }

        GUILayout.EndArea();

        GUI.matrix = Matrix4x4.identity;
    }

    void OnGUI()
    {
        GUI.depth = 0;
        GUI.skin = hintSkin;

        if (activeHint || (!activeHint && tScale != new Vector3(0.1f, 0.1f, 0.1f)))
        {
            DrawHintsWindow(currentState, delaySlideTime);
        }
    }
}
