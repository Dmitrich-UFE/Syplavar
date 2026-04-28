using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.IO;

public class VNLDialogueWindow : MonoBehaviour
{
    [SerializeField] private GameObject _DialogueWindow;
    [SerializeField] private GameObject _NameObject;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private CanvasGroup _dialogueWindowCanvas;
    [SerializeField] private float fadeSpeed;
    [SerializeField] private float printSpeed;

    private WaitForSecondsRealtime tick;
    private WaitForSecondsRealtime printtick;
    private List<string> assetStrings;
    private string pattern = @">(?<key>.*?)>\s*""(?<value>.*)""";
    private Queue<string> cuttedLetters;
    private PlayerInputActions _playerInputActions;


    void Awake()
    {
        tick = new WaitForSecondsRealtime(fadeSpeed > 0 ? fadeSpeed : 0.01f);
        printtick = new WaitForSecondsRealtime(printSpeed > 0 ? fadeSpeed : 0.01f);
        cuttedLetters = new Queue<string>(); 
        _playerInputActions = new PlayerInputActions();
    }

    internal void StartPrint(TextAsset textAsset)
    {
        assetStrings.Clear();

        using (StringReader reader = new StringReader(textAsset.text))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                assetStrings.Add(line);
            }
        }

        
    }

    private IEnumerator PrintText()
    {
        foreach (string assetString in assetStrings)
        {
            cuttedLetters.Clear();
            (string name, string text) parsedAssetString = ParseTextAssetString(assetString);
            cuttedLetters = CutStringByLetters(parsedAssetString.text);

            if (parsedAssetString.name != "") 
            { _NameObject.SetActive(true); nameText.text = parsedAssetString.name; }
            else { _NameObject.SetActive(false); }

            dialogueText.text = "";

            while(cuttedLetters.Count != 0)
            {
                dialogueText.text += cuttedLetters.Dequeue();
                yield return printtick;
            }

            //TODO: дописать далее переход на следующую строку через кастомный yield

        }
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

        while (_dialogueWindowCanvas.alpha < 1f)
        {
            _dialogueWindowCanvas.alpha+=0.04f;
            yield return tick;
        }
    }

    private IEnumerator  FadeOutWindow()
    {
        _dialogueWindowCanvas.alpha = 1f;

        while (_dialogueWindowCanvas.alpha > 0f)
        {
            _dialogueWindowCanvas.alpha-=0.04f;
            yield return tick;
        }

        _DialogueWindow.SetActive(false);
    }

    private void OnEnable()
    {
        _playerInputActions.Enable();
    }

    private void OnDisable()
    {
        _playerInputActions.Disable();
    }
}
