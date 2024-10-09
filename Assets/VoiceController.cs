using Meta.WitAi;
using Meta.WitAi.Configuration;
using Meta.WitAi.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

public class VoiceController : MonoBehaviour
{
    public VoiceService voiceService;

    private VoiceServiceRequest voiceServiceRequest;
    private Coroutine recordingLoop;
    private List<string> lastWords = new List<string>();

    private void Start()
    {
        if (voiceService != null)
        {
            voiceService.VoiceEvents.OnPartialTranscription.AddListener(SendTranscription);
        }
        StartRecording();
    }

    private void OnDestroy()
    {
        if (voiceService != null)
        {
            voiceService.VoiceEvents.OnPartialTranscription.RemoveListener(SendTranscription);
        }
    }

    private void SendTranscription(string text)
    {
        string cleanText = Regex.Replace(text.Normalize(NormalizationForm.FormD), @"[^a-zA-Z\s]", "").ToLower();
        string[] allWords = cleanText.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
        List<string> newWords = new List<string>();
        foreach (string word in allWords)
        {
            if (!lastWords.Contains(word))
            {
                newWords.Add(word);
            }
        }
        lastWords = allWords.ToList();
        //CPRTree.GetInstance().NewWords(newWords);
        foreach (string s in newWords)
        {
            print("Palabras:" + s);
        }
    }

    public void StartRecording()
    {
        if (recordingLoop == null)
        {
            recordingLoop = StartCoroutine(RecordingLoop());
        }
    }

    public void EndRecording()
    {
        if (recordingLoop != null)
        {
            StopCoroutine(recordingLoop);
            recordingLoop = null;
            if (voiceServiceRequest != null)
            {
                voiceServiceRequest.Cancel();
            }
        }
    }

    private IEnumerator RecordingLoop()
    {
        while (true)
        {
            voiceServiceRequest = voiceService.Activate(new WitRequestOptions(), new VoiceServiceRequestEvents());
            yield return new WaitUntil(() => VoiceEnded());
        }
    }

    private bool VoiceEnded()
    {
        return voiceServiceRequest != null && (voiceServiceRequest.State == Meta.Voice.VoiceRequestState.Successful || voiceServiceRequest.State == Meta.Voice.VoiceRequestState.Failed || voiceServiceRequest.State == Meta.Voice.VoiceRequestState.Canceled);
    }
}
