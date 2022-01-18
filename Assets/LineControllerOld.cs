using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LineControllerOld : MonoBehaviour
{
    public enum AnimationType{None,SingleColorMorph,MultiColorMorph,Shuffle,Shift};
    public AnimationType myAnimationType;
    LineRenderer myLineRenderer;
    public float morphTime = 2;
    public Gradient gradient;
    public Gradient gradient2;
    public Gradient grdient3;
    public Gradient gradient4;
    public Color firstColor;
    public Color lastColor;
    public Color newIntersectionColor;
    public float colorLerpAmount;
    // Start is called before the first frame update
    void Awake()
    {

        myLineRenderer = this.GetComponent<LineRenderer>();

    }
    void Start()
    {
        // List<GradientColorKey> testList = new List<GradientColorKey>(gradient.colorKeys);
        // Debug.Log(gradient.colorKeys[0]);
        // testList.Add(new GradientColorKey(Color.black, 0));
        // GradientColorKey holder = testList[0];
        // testList[0] = testList[testList.Count-1];
        // testList[testList.Count-1] = holder;
        // gradient.colorKeys = testList.ToArray(); //puts the colorkeys in order from lowest to highest time.
        // testList = new List<GradientColorKey>(gradient.colorKeys);
        // GradientColorKey colorKeyToChange = testList[0];
        // colorKeyToChange.time += .2f;
        // testList[0] = colorKeyToChange;
        // gradient.colorKeys = testList.ToArray();

        // List<GradientColorKey> testList = new List<GradientColorKey>(gradient.colorKeys);
        // Debug.Log(testList[0].time);
        // GradientColorKey tempKey = testList[0];
        // tempKey.time = .4f;
        // testList[0] = tempKey;
        // Debug.Log(testList[0].time);
        // Gradient newG = new Gradient();
        // newG.colorKeys = testList.ToArray();
        // Debug.Log(testList[0].time);
        // Debug.Log(newG.colorKeys[0].time);
        switch(myAnimationType)
        {
            case AnimationType.SingleColorMorph:
                StartCoroutine(RandomSingleColorMorphing(myLineRenderer,morphTime));
                break;
            case AnimationType.MultiColorMorph:
                StartCoroutine(RandomMultiColorMorphing(myLineRenderer,morphTime));
                break;
            case AnimationType.Shuffle:
                StartCoroutine(ShuffleGradient(myLineRenderer, .5f));
                break;
            case AnimationType.Shift:
                StartCoroutine(ShiftLoopRight(myLineRenderer));
                break;
        }
    }
    void Update()
    {

    }


    IEnumerator ShuffleGradient(LineRenderer targetLineRenderer, float waitTime)
    {
        while(true)
        {
            ShuffleGradient(targetLineRenderer);
            yield return new WaitForSeconds(waitTime);
        }
    }
    void ShuffleGradient(LineRenderer targetLineRenderer)
    {
        GradientColorKey[] newColorKeys = targetLineRenderer.colorGradient.colorKeys;
        for(int i = 0 ; i <newColorKeys.Length; i ++)
        {
            Color tempColor = newColorKeys[i].color;
            int randomIndex = Random.Range(0,newColorKeys.Length-1);
            newColorKeys[i].color = newColorKeys[randomIndex].color;
            newColorKeys[randomIndex].color = tempColor;
        }
        Gradient tempGradient = targetLineRenderer.colorGradient;
        tempGradient.colorKeys = newColorKeys;
        targetLineRenderer.colorGradient = tempGradient;
    }
    IEnumerator RandomMultiColorMorphing(LineRenderer lineRendererToChange, float timeToMorph)
    {
        float time = 0;

        while(true)
        {
            GradientColorKey[] initialColorKeys = lineRendererToChange.colorGradient.colorKeys;
            GradientColorKey[] newColorKeys = GenerateRandomColorKeys(initialColorKeys);
            time = 0;
            while(time<timeToMorph)
            {
                time += Time.deltaTime;
                float progress = time/timeToMorph;
                GradientColorKey[] currentColorKeys = GradientColorKeyLerp(initialColorKeys,newColorKeys,progress);
                Gradient tempGradient = lineRendererToChange.colorGradient;
                tempGradient.colorKeys = currentColorKeys;
                lineRendererToChange.colorGradient = tempGradient;
                yield return null;
            }
            yield return null;
        }
    }
    
    GradientColorKey[] GradientColorKeyLerp(GradientColorKey[] initialColorKeys, GradientColorKey[] endColorKeys, float progress)
    {
        GradientColorKey[] newColorKeys = new GradientColorKey[initialColorKeys.Length];
        for(int i = 0; i < newColorKeys.Length; i++)
        {
            newColorKeys[i].color = Color.Lerp(initialColorKeys[i].color, endColorKeys[i].color, progress);
            newColorKeys[i].time = initialColorKeys[i].time;
        }
        return newColorKeys;
    }
    //assigns new color to each colorkey and uses Time from incomingColorKeys
    GradientColorKey[] GenerateRandomColorKeys(GradientColorKey[] incomingColorKeys)
    {
        GradientColorKey[] newColorKeys = new GradientColorKey[incomingColorKeys.Length];
        for(int i = 0; i < newColorKeys.Length ; i++)
        {
            newColorKeys[i].color = RandomColor();
            newColorKeys[i].time = incomingColorKeys[i].time;
        }
        return newColorKeys;
    }

    //asumes Single color, 2 colorkeys
    IEnumerator RandomSingleColorMorphing(LineRenderer lineRendererToChange, float timeToMorph)
    {
        
        float time = 0;
        Color initialColor = lineRendererToChange.colorGradient.colorKeys[0].color;

        //this makes sure to reduce colorkeys amount to 2 in case there isn't
        //also makes the line Singlecolor
        SetSingleColor2(lineRendererToChange,initialColor); 

        while(true)
        {
            initialColor = lineRendererToChange.colorGradient.colorKeys[0].color;
            Color targetColor = RandomColor();
            time = 0;
            while(time<timeToMorph)
            {
                time+=Time.deltaTime;
                float progress = time/timeToMorph;
                Color currentColor = Color.Lerp(initialColor,targetColor,progress);
                SetSingleColor(lineRendererToChange,currentColor);
                yield return null;
            }
            yield return null;
        }
    }

    //This only works if there's only 2 colorKeys in your lineRendererToChange color gradient
    void SetSingleColor(LineRenderer lineRendererToChange, Color newColor)
    {
        lineRendererToChange.startColor = newColor;
        lineRendererToChange.endColor = newColor;
    }

    //This makes sure there's only 2 colorKeys in your lineRendererToChange and sets them to the newColor
    void SetSingleColor2(LineRenderer lineRendererToChange, Color newColor, float alpha =1)
    {
        Gradient tempGradient = new Gradient();

        GradientColorKey[] tempColorKeys = new GradientColorKey[2];
        tempColorKeys[0] = new GradientColorKey(newColor,0);
        tempColorKeys[1] = new GradientColorKey(newColor,1);

        tempGradient.colorKeys = tempColorKeys;

        lineRendererToChange.colorGradient = tempGradient;
    }

    //this keeps the same amount of colorkeys and sets them all to the new color, while keeping the same alphaKeys
    void SetSingleColor3(LineRenderer lineRendererToChange, Color newColor)
    {
        Gradient tempGradient = lineRendererToChange.colorGradient;

        GradientColorKey[] tempColorKeys = tempGradient.colorKeys;
        for(int i =0 ; i < tempColorKeys.Length; i++)
        {
            tempColorKeys[i].color = newColor;
        }
        tempGradient.colorKeys = tempColorKeys;

        lineRendererToChange.colorGradient = tempGradient;
    }



    void ShiftColorLeft(float speed)
    {

    }
    void ShiftColorKeyLeft()
    {
        Gradient initialGradient = myLineRenderer.colorGradient;
        GradientColorKey[] initialColorKeys = initialGradient.colorKeys;
        GradientColorKey[] newColorKeys = new GradientColorKey[initialColorKeys.Length];
        for(int i = 0 ; i < initialColorKeys.Length ; i ++ )
        {
            int nextIndex = GetNextIndex(i,initialColorKeys.Length-1);
            newColorKeys[i].color = initialColorKeys[nextIndex].color;
            newColorKeys[i].time = initialColorKeys[i].time;
            Debug.Log(newColorKeys[i].color);
            // newColorKeys[i] = new GradientColorKey(newColor,initialColorKeys[i].time);
            // if(i == initialColorKeys.Length-1)
            // {
            //     newColorKeys[i].color = initialColorKeys[0].color;
            // }
        }
        Gradient newGradient = new Gradient();
        newGradient.colorKeys = newColorKeys;
        newGradient.alphaKeys = initialGradient.alphaKeys;
        Debug.Log(newGradient.colorKeys.Length);
        myLineRenderer.colorGradient = newGradient;
    }
    int GetNextIndex(int currentIndex, int maxIndex)
    {
        if(currentIndex==maxIndex)
        return 0;
        else
        return currentIndex+1;
    }
    Color RandomColor()
    {
        return new Color(Random.Range(0f,1f),Random.Range(0f,1f),Random.Range(0f,1f));
    }
    IEnumerator ShiftLoopRight(LineRenderer lineRendererToChange, float movementPerTick = .01f)
    {
        GradientColorKey[] initialColorKeys = lineRendererToChange.colorGradient.colorKeys;
        List<GradientColorKey> newColorKeys = new List<GradientColorKey>(lineRendererToChange.colorGradient.colorKeys);
        Color interSectionColor = initialColorKeys[0].color;
        //makes a copy of first and last point so those can move while keeping fixed end and start to be able to gradient over intersection
        newColorKeys.Insert(0,new GradientColorKey(interSectionColor,0));
        // newColorKeys.Add(new GradientColorKey(interSectionColor,1));
        Gradient newInitGradient = new Gradient();
        newInitGradient.colorKeys = newColorKeys.ToArray();
        lineRendererToChange.colorGradient = newInitGradient;
        while(true)
        {
            if(Input.GetKey(KeyCode.A))
            {
                gradient = lineRendererToChange.colorGradient;

                    //first key, same position, new Color which will be based between first and last colors.
                //tempkeys can be kept track of instead of reimporting from line's gradient
                GradientColorKey[] tempKeys = lineRendererToChange.colorGradient.colorKeys;
                //first and secondtolast are the fixed colors, the copy that will move are 2nd and last due to how initializing keys orders them.
                List<GradientColorKey> currentColorKeys = new List<GradientColorKey>(lineRendererToChange.colorGradient.colorKeys);
                Debug.Log(tempKeys[currentColorKeys.Count-1].color == currentColorKeys[currentColorKeys.Count-1].color);
                //remove first and second to last keys to work with all normal shfiting ones.
                currentColorKeys.RemoveAt(currentColorKeys.Count-1);
                currentColorKeys.RemoveAt(0);

                Gradient niuG = new Gradient();
                niuG.colorKeys = currentColorKeys.ToArray();
                gradient2 = niuG;
                // Debug.Log(tempKeys.Length + " " + currentColorKeys.Count);
                // Debug.Log(tempKeys[0].color +" " + currentColorKeys[0].color);
                // Debug.Log(tempKeys[1].color + " " +currentColorKeys[1].color);
                // Debug.Log(tempKeys[2].color + " " +currentColorKeys[2].color);

                float highestTime=0;
                float lowestTime=1;
                int highestIndex = currentColorKeys.Count-1;
                int lowestIndex = 0;
                //Move all inner ones.
                for(int i = 0 ;i<currentColorKeys.Count;i++)
                {
                    GradientColorKey tempColorKey = currentColorKeys[i];
                    float newTime = tempColorKey.time + movementPerTick;
                    if(newTime>1)
                    {
                        newTime = newTime-1;
                    }
                    tempColorKey.time = newTime;
                    currentColorKeys[i] = tempColorKey;
                    if(newTime<lowestTime)
                    {
                        lowestTime = newTime;
                        lowestIndex = i;
                    }
                    if(newTime>highestTime)
                    {
                        highestTime = newTime;
                        highestIndex = i;
                    }
                    yield return null;
                }
                Debug.Log(lowestIndex + " " + highestIndex);
                firstColor = currentColorKeys[lowestIndex].color;
                lastColor = currentColorKeys[highestIndex].color;
                float distance = 1 - (currentColorKeys[highestIndex].time - currentColorKeys[lowestIndex].time);
                colorLerpAmount = (1f-currentColorKeys[highestIndex].time) / distance;
                // Debug.Log(firstColor + " " + lastColor);
                newIntersectionColor = Color.Lerp(lastColor,firstColor,colorLerpAmount);

                Gradient otherNewG = new Gradient();
                otherNewG.colorKeys = currentColorKeys.ToArray();
                grdient3 = otherNewG;
                //change color of first and last
                //reinject first and last
                currentColorKeys.Insert(0,new GradientColorKey(newIntersectionColor,0));
                currentColorKeys.Add(new GradientColorKey(newIntersectionColor,1));
                // Debug.Log(newIntersectionColor);
                Gradient tempGradient = lineRendererToChange.colorGradient;
                tempGradient.colorKeys = currentColorKeys.ToArray();
                lineRendererToChange.colorGradient = tempGradient;  
                gradient4 = tempGradient;
            }
            yield return null;
        }
        // yield return null;
    }
}
