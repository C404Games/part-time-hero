using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HistoryManager : MonoBehaviour
{
    public class HistoryAction
    {
        public List<string> data = new List<string>();
        public string type;
        public string character;
        public int question;

        public HistoryAction()
        {
        }
    }

    public AudioSource backgroundMusic;
    public AudioSource forestgroundMusic;
    public AudioSource voice;
    public List<HistoryAction> actions = new List<HistoryAction>();
    public enum Action {PROLOGUE, DIALOGUE, POWERUP, MONSTER, QUESTION, WRITING, BACKGROUND};
    private UnityEngine.UI.Text globalText;
    private UnityEngine.UI.RawImage character1RawImage;
    private UnityEngine.UI.RawImage option1Selected;
    private UnityEngine.UI.RawImage option2Selected;
    private UnityEngine.UI.RawImage characterSpeaker;
    private UnityEngine.UI.RawImage conversationBox;
    private UnityEngine.UI.Text textOption1;
    private UnityEngine.UI.Text textOption2;
    private UnityEngine.UI.Text characterNameText;
    private UnityEngine.UI.Text town1Text;
    private UnityEngine.UI.Text town1DescriptionText;
    private UnityEngine.UI.Text town2Text;
    private UnityEngine.UI.Text town2DescriptionText;
    private UnityEngine.UI.Text prologueText;
    private UnityEngine.UI.Button nextButton;
    private UnityEngine.UI.Button option1Button;
    private UnityEngine.UI.Button option2Button;
    private UnityEngine.UI.Button town1Button;
    private UnityEngine.UI.Button town2Button;
    public UnityEngine.UI.Text nextButtonText;
    public UnityEngine.UI.InputField nextInputField;
    private string characterName;
    private Animator historyAnimator;
    private int currentStep;
    private int currentConversationStep;
    private int selectedQuestion;
    private string town1Description;
    private string town2Description;
    private string town1Name;
    private string town2Name;
    private bool prologueActive;
    private bool musicActive;
    private bool nextStepIsTaken;
    private string language;
    private IEnumerator fadeCorroutine;
    public Texture backgroundTexture;
    public Texture barkeeperTexture;
    public Texture blacksmithTexture;
    public Texture unknownTexture;
    public Texture magicTexture;
    public Texture town1;
    public Texture town2;
    public int episode;
    private string place;

    // Start is called before the first frame update
    void Awake()
    {
        language = PlayerPrefs.GetString("language","Spanish");
        place = "BOSQUE";
        town1Name = "TOMATELANDIA";
        town2Name = "PATATALANDIA";
        town1Description = "Ciudad que te proporcionará un bonus de experiencia y emociones que te permitirá mejorar rápidamente tu rango social";
        town2Description = "Ciudad que te proporcionará un bonus de dinero que podrás usar para mejorar tu apariencia y conseguir tesoros";
        historyAnimator = GetComponent<Animator>();
        UnityEngine.UI.RawImage[] rawImages = GetComponentsInChildren<UnityEngine.UI.RawImage>();
        UnityEngine.UI.Button[] buttons = GetComponentsInChildren<UnityEngine.UI.Button>();
        UnityEngine.UI.Text[] texts = GetComponentsInChildren<UnityEngine.UI.Text>();
        UnityEngine.UI.InputField[] inputFields = GetComponentsInChildren<UnityEngine.UI.InputField>();
        character1RawImage = rawImages[2];
        globalText = texts[1];
        textOption1 = texts[5];
        textOption2 = texts[6];
        town1Text = texts[7];
        town1DescriptionText = texts[8];
        town2Text = texts[9];
        town2DescriptionText = texts[10];
        prologueText = texts[11];
        town1Text.text = town1Name;
        town2Text.text = town2Name;
        town1DescriptionText.text = town1Description;
        town2DescriptionText.text = town2Description;
        rawImages[7].texture = town1;
        rawImages[8].texture = town2;
        characterNameText = texts[4];
        nextButton = buttons[0];
        nextInputField = inputFields[0];
        rawImages[0].texture = backgroundTexture;
        conversationBox = rawImages[2];
        characterSpeaker = rawImages[3];
        option1Selected = rawImages[5];
        option2Selected = rawImages[6];
        option1Button = buttons[1];
        option2Button = buttons[2];
        town1Button = buttons[3];
        town2Button = buttons[4];
        nextButton.onClick.AddListener(delegate { nextHistoryStep(0); });
        option1Button.onClick.AddListener(delegate { nextHistoryStep(1); });
        option2Button.onClick.AddListener(delegate { nextHistoryStep(2); });
        town1Button.onClick.AddListener(delegate { townSelection(0); });
        town2Button.onClick.AddListener(delegate { townSelection(1); });
        TextAsset mytxtData = (TextAsset)Resources.Load("es-story1");
        string[] readText = mytxtData.text.Split('\n');
        //string[] readText = File.ReadAllLines(Application.dataPath + "/es-story1.txt");
        if (language == "Spanish")
        {
            nextButtonText.text = "Siguiente...";
            nextInputField.placeholder.GetComponent<UnityEngine.UI.Text>().text = "Introduce el nombre...";
            mytxtData = (TextAsset)Resources.Load("es-story" + episode );
            readText = mytxtData.text.Split('\n');
        } else if (language == "English")
        {
            nextButtonText.text = "Next...";
            nextInputField.placeholder.GetComponent<UnityEngine.UI.Text>().text = "Write your name...";
            mytxtData = (TextAsset)Resources.Load("en-story" + episode);
            readText = mytxtData.text.Split('\n');
        } else if (language == "Simplified Chinese")
        {
            nextButtonText.text = "下列的...";
            nextInputField.placeholder.GetComponent<UnityEngine.UI.Text>().text = "寫你的名字";
            mytxtData = (TextAsset)Resources.Load("cn-story" + episode);
            readText = mytxtData.text.Split('\n');
        }
        foreach (string s in readText)
        {
            if (s.Contains("PRE"))
            {
                HistoryAction action = new HistoryAction();
                action.data.Add(s.Split(':')[1]);
                action.type = Action.PROLOGUE.ToString();
                actions.Add(action);
            }
            else if (s.Contains("-"))
            {
                HistoryAction action = new HistoryAction();
                action.data.Add(s.Split(':')[1]);
                action.character = s.Split(':')[0].Split('-')[1].Split('(')[0];
                if (s.Split(':')[1].Contains("Introduce el nombre"))
                {
                    action.type = Action.WRITING.ToString();
                } else if (s.Split(':')[1].Contains("opciones de dialogo"))
                {
                    action.type = Action.QUESTION.ToString();
                } else
                {
                    action.type = Action.DIALOGUE.ToString();
                }
                if (s.Contains("(P1)"))
                {
                    action.question = 1;
                } else if (s.Contains("(P2)"))
                {
                    action.question = 2;
                } else
                {
                    action.question = 0;
                }
                actions.Add(action);

            } else if (s.Contains("="))
            {
                HistoryAction action = new HistoryAction();
                action.type = Action.BACKGROUND.ToString();
                action.data.Add(s.Split('=')[1]);
                actions.Add(action);
            } else {
                HistoryAction action = actions[actions.Count - 1];
                action.data.Add(s.Split(':')[1]);
            }
        }
    }

    void Start()
    {
        backgroundMusic.Play();
        if (language == "Spanish"){
            voice.Play();
        }
        fadeCorroutine = fadeOutScene(2.5f);
        prologueActive = true;
        musicActive = false;
        prologueText.text = actions[currentStep].data[0];
        nextInputField.transform.localScale = new Vector3(0, 0, 0);
        option1Selected.transform.localScale = new Vector3(0, 0, 0);
        option2Selected.transform.localScale = new Vector3(0, 0, 0);
        characterSpeaker.transform.localScale = new Vector3(0, 0, 0);
        if (language == "Spanish")
        {
            if ( episode == 1) {
                globalText.text = "(Ufff ya llegué a mi destino, creo que este hombre que se acerca me ayudará a encontrar lo que buscaba.)";
            } else if ( episode == 2) {
                globalText.text = "(Mmmm ¡Qué bien huele! Qué hambre me está entrando.)";
            } else if ( episode == 3) {
                globalText.text = "(¡Vaya! Qué ordenado está todo.)";
            }

        } else if (language == "English")
        {
            if ( episode == 1) {
                globalText.text = "(Ufff I finally arrive to this civilization, I think that man will help me about what I'm looking for.)";
            } else if ( episode == 2) {
                globalText.text = "(Mmmm Smells so good! I'm hungry.)";
            } else if ( episode == 3) {
                globalText.text = "(Wow! It's very tidy!)";
            }
        } else if (language == "Simplified Chinese")
        {
            if ( episode == 1) {
                globalText.text = "(我已經到達目的地了，我認為這個正在接近的人將幫助我找到想要的東西。)";
            } else if ( episode == 2) {
                globalText.text = "(嗯，闻起来好香啊! 我很饿。)";
            } else if ( episode == 3) {
                globalText.text = "(哇! 非常整洁!)";
            }
        }

        textOption1.text = "";
        textOption2.text = "";
        currentConversationStep = 0;
        currentStep = 0;
    }

    void Update()
    {
        if (prologueActive)
        {
            if (historyAnimator.GetCurrentAnimatorStateInfo(0).IsName("textShowIdle"))
            {
                if (!nextStepIsTaken)
                {
                    nextStepIsTaken = true;
                    currentStep++;
                }
            }
            if (historyAnimator.GetCurrentAnimatorStateInfo(0).IsName("textFadeOut"))
            {
                nextStepIsTaken = false;
                if (actions[currentStep].type != Action.PROLOGUE.ToString())
                {
                    prologueActive = false;
                    historyAnimator.SetBool("movingTree",true);
                }
            }
            if (historyAnimator.GetCurrentAnimatorStateInfo(0).IsName("textFadeIn"))
            {
                prologueText.text = actions[currentStep].data[0];
            }
        } else
        {
            backgroundMusic.Stop();
            if (!musicActive ){
                musicActive = true;
                forestgroundMusic.Play();
            }
            if ((nextInputField.transform.localScale[0] == 1 && nextInputField.text.ToString().Length == 0 ) || actions[currentStep].type == Action.QUESTION.ToString() && currentConversationStep == 1)
            {
                nextButton.interactable = false;
            }
            else
            {
                nextButton.interactable = true;
            }
        }        
    }

    public void nextHistoryStep(int button)
    {
        HistoryAction action = actions[currentStep];
        string type = action.type;
        if ((currentStep+1) == actions.Count && (currentConversationStep + 1) == actions[currentStep].data.Count)
        {
            PlayerPrefs.SetString("characterPlayerName", nextInputField.text);
            prologueText.text = "";
            historyAnimator.SetTrigger("finishDialogue");
            StartCoroutine(fadeCorroutine);
        } else
        {
            if (globalText.text.Contains("Ufff ya llegué a mi destino") || globalText.text.Contains("Ufff I finally arrive to this civilization") ||
                globalText.text.Contains("我已經到達目的地了"))
            {
                globalText.text = "";
                nextHistoryStep(button);
            } else
            {
                if (action.type == Action.BACKGROUND.ToString())
                {
                    currentStep++;
                    action = actions[currentStep];
                    type = action.type;
                    currentConversationStep = 0;
                    Debug.Log("Cambio de escenario:" + action.data[currentStep]);
                }
                if (action.character != "Player")
                {
                    characterNameText.text = action.character;
                    switch (action.character)
                    {
                        case "Kamerlín":
                            {
                                characterSpeaker.transform.localScale = new Vector3(1.8f, 1.8f, 1);
                                characterSpeaker.texture = magicTexture;
                                break;
                            }
                        case "Tario":
                            {
                                characterSpeaker.transform.localScale = new Vector3(1.2f, 2.5f, 1);
                                characterSpeaker.texture = barkeeperTexture;
                                break;
                            }
                        case "Rhiron":
                            {
                                characterSpeaker.transform.localScale = new Vector3(1.4f, 2, 1);
                                characterSpeaker.texture = blacksmithTexture;
                                break;
                            }
                        case "???":
                            {
                                characterSpeaker.transform.localScale = new Vector3(1.8f, 1.8f, 1);
                                characterSpeaker.texture = magicTexture;
                                break;
                            }
                    }
                }
                if (nextInputField.transform.localScale[0] == 1)
                {
                    characterName = nextInputField.text.ToString();
                    nextInputField.transform.localScale = new Vector3(0, 0, 0);
                }
                if (textOption1.text != "")
                {
                    selectedQuestion = button;
                    for (int i = currentStep; i < actions.Count; i++)
                    {
                        if (actions[i].question == button)
                        {
                            currentStep = i;
                            currentConversationStep = 0;
                            nextButton.transform.localScale = new Vector3(1, 1, 1);
                            option1Button.transform.localScale = new Vector3(0, 0, 0);
                            option2Button.transform.localScale = new Vector3(0, 0, 0);
                            action = actions[currentStep];
                            type = action.type;
                            break;
                        }
                    }
                }
                if (type == Action.DIALOGUE.ToString())
                {
                    globalText.text = actions[currentStep].data[currentConversationStep];
                    textOption1.text = "";
                    textOption2.text = "";
                    nextButton.interactable = true;
                    option1Button.transform.localScale = new Vector3(0, 0, 0);
                    option2Button.transform.localScale = new Vector3(0, 0, 0);
                }
                else if (type == Action.QUESTION.ToString())
                {
                    globalText.text = "";
                    textOption1.text = actions[currentStep].data[1];
                    textOption2.text = actions[currentStep].data[2];
                    nextButton.interactable = false;
                    option1Button.transform.localScale = new Vector3(1, 1, 1);
                    option2Button.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (type == Action.WRITING.ToString())
                {
                    globalText.text = "";
                    textOption1.text = "";
                    textOption2.text = "";
                    nextButton.interactable = false;
                    nextInputField.transform.localScale = new Vector3(1, 1, 1);
                    option1Button.transform.localScale = new Vector3(0, 0, 0);
                    option2Button.transform.localScale = new Vector3(0, 0, 0);
                }
                currentConversationStep++;
                if (action.data.Count == currentConversationStep)
                {
                    currentConversationStep = 0;
                    if (actions.Count > (currentStep + 1))
                    {
                        currentStep++;
                    }
                }
            }            
        }        
    }

    public void townSelection(int town)
    {
        if (town == 0)
        {
            PlayerPrefs.SetString("town", "Tomatelandia");
            PlayerPrefs.SetString("characterPlayerName", this.characterName);
            PlayerPrefs.SetInt("storyLevelAvailable", 1);
            PlayerPrefs.SetInt("tutorialActive", 0);
            this.historyAnimator.SetTrigger("town1Selected");
            StartCoroutine(fadeCorroutine);
        } else if (town == 1)
        {
            PlayerPrefs.SetString("town", "Patatalandia");
            PlayerPrefs.SetString("characterPlayerName", this.characterName);
            PlayerPrefs.SetInt("storyLevelAvailable", 1);
            PlayerPrefs.SetInt("tutorialActive", 0);
            this.historyAnimator.SetTrigger("town2Selected");
            StartCoroutine(fadeCorroutine);
        }
    }

    public IEnumerator fadeOutScene(float time)
    {
        backgroundMusic.Stop();
        yield return new WaitForSeconds(time);
        SceneManager.LoadScene("MainMenu");
    }
}
