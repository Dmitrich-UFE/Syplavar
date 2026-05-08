using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class VNLDialogueWindow : MonoBehaviour
{
    [Header("Поля элементов диалога VNL")]
    [SerializeField] private GameObject _DialogueWindow;
    [SerializeField] private GameObject _NameObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private GameObject EoSIndicator;
    [SerializeField] private CanvasGroup _dialogueWindowCanvas;

    [Header("Параметры вывода/анимаций")]
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float printSpeed;

    [Header("Выключаемые элементы при активном диалоге")]
    [SerializeField] private GameObject lowerInventory;
    [SerializeField] private GameObject gameCharacteristics;
    [SerializeField] private GameObject uiHandler;
    [SerializeField] private Movement movement;
    [SerializeField] private GameObject cursor;
    [SerializeField] private GameObject storyUI;

    [Header("Прочее")]
    [SerializeField] private TextAsset textAsset;



    private WaitForSecondsRealtime fadetick;
    private WaitForSecondsRealtime printtick;
    private List<string> assetStrings;
    private string pattern = @">(?<key>.*?)>\s*""(?<value>.*)""";
    private Queue<string> cuttedLetters;
    private PlayerInputActions _playerInputActions;
    private Coroutine fadeCoroutine;
    private bool IsReadyForPrint = false;
    private bool _skipRequested;

    internal VNLprintStatus Status {get; private set;}


    void Awake()
    {
        fadetick = new WaitForSecondsRealtime(fadeSpeed != 0 ? 1 / fadeSpeed : 0.01f);
        printtick = new WaitForSecondsRealtime(printSpeed != 0 ? 1 / printSpeed : 0.02f);

        cuttedLetters = new Queue<string>(); 
        assetStrings = new  List<string>();

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.SkipDialogue.performed += _ => _skipRequested = true;
    }

    void Start()
    {
        //StartPrint(textAsset);
    }

    //Подготовка к печати
    internal void StartPrint(TextAsset textAsset)
    {
        Status = VNLprintStatus.started;
        dialogueText.text = "";
        assetStrings.Clear();
        EoSIndicator.SetActive(false);

        using (StringReader reader = new StringReader(textAsset.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                assetStrings.Add(line);
            }
        }

        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        fadeCoroutine = StartCoroutine(FadeInWindow());

        StartCoroutine(PrintText());
    }

    //Процесс печати 
    private IEnumerator PrintText()
    {
        while (!IsReadyForPrint)
        {
            yield return fadetick;
        }

        int index = 0;

        foreach (string assetString in assetStrings)
        {
            if (index == 0) {Status = VNLprintStatus.first;}
            if (index == assetStrings.Count - 1) {Status = VNLprintStatus.last;}

            cuttedLetters.Clear();
            EoSIndicator.SetActive(false);
            _skipRequested = false;

            (string name, string text) parsedAssetString = ParseTextAssetString(assetString);
            cuttedLetters = CutStringByLetters(parsedAssetString.text);

            if (parsedAssetString.name != "") 
            { _NameObject.SetActive(true); nameText.text = parsedAssetString.name; }
            else { _NameObject.SetActive(false); }

            dialogueText.text = "";

            
            while(cuttedLetters.Count > 0 && !_skipRequested)
            {
                dialogueText.text += cuttedLetters.Dequeue();
                yield return printtick;
            }

            if (cuttedLetters.Count > 0)
            {
                dialogueText.text += string.Join("", cuttedLetters.ToArray());
                cuttedLetters.Clear();
                _skipRequested = false;
            }
            
            EoSIndicator.SetActive(true);

            yield return null;
            yield return new WaitForInputAction(_playerInputActions.Player.SkipDialogue);
        }

        EndPrint();
    }

    //закрытие окна
    private void EndPrint()
    {
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        Status = VNLprintStatus.ended;
        fadeCoroutine = StartCoroutine(FadeOutWindow());
    }


    //ПАРСИТ СТРОКУ НА ИМЯ И ТЕКСТ
    (string name, string text) ParseTextAssetString(string line)
    {
        Match match = Regex.Match(line, pattern);

        if (match.Success)
        {
            string key = match.Groups["key"].Value;     // СТРОКА1
            string value = match.Groups["value"].Value; // СТРОКА2

            return (key, value);
        }

        return ("", "");
    }

    //Режет строку на буквы и теги
    private Queue<string> CutStringByLetters(string Sentence)
    {
        Queue<string> SymbolsQueue = new Queue<string>();
        MatchCollection Symbols = Regex.Matches(Sentence, @"(<[^>]+>|.)");

        foreach (Match Symbol in Symbols) { SymbolsQueue.Enqueue(Symbol.Value); }

        return SymbolsQueue;
    }


    //АНИМАЦИИ
    private IEnumerator FadeInWindow()
    {
        _dialogueWindowCanvas.alpha = 0f;
        _DialogueWindow.SetActive(true);
        SetOffComponentsBeforeDialogue();

        while (_dialogueWindowCanvas.alpha < 1f)
        {
            _dialogueWindowCanvas.alpha+=0.04f;
            yield return fadetick;
        }

        IsReadyForPrint = true;
    }

    private IEnumerator  FadeOutWindow()
    {
        IsReadyForPrint = false;
        _dialogueWindowCanvas.alpha = 1f;

        while (_dialogueWindowCanvas.alpha > 0f)
        {
            _dialogueWindowCanvas.alpha-=0.04f;
            yield return fadetick;
        }

        _DialogueWindow.SetActive(false);
        //Status = VNLprintStatus.nil;
        SetOnComponentsAfterDialogue();
    }

    private void SetOffComponentsBeforeDialogue()
    {
        uiHandler.SetActive(false);
        cursor.SetActive(false);
        movement.enabled = false;
        lowerInventory.SetActive(false);
        gameCharacteristics.SetActive(false);
        storyUI.SetActive(false);
        Time.timeScale = 0f;
    }

    private void SetOnComponentsAfterDialogue()
    {
        uiHandler.SetActive(true);
        cursor.SetActive(true);
        movement.enabled = true;
        lowerInventory.SetActive(true);
        gameCharacteristics.SetActive(true);
        storyUI.SetActive(true);
        Time.timeScale = 1f;
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
        _playerInputActions.Player.SkipDialogue.performed += _ => _skipRequested = true;
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}

public enum VNLprintStatus {started, first, last, ended, nil}
