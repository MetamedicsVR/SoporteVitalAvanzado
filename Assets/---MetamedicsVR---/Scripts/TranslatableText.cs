using System;

[Serializable]
public class TranslatableText
{
    public string englishText;
    public string spanishText;

    public string GetTranslation()
    {
        return GetTranslation(LanguageManager.GetInstance().GetCurrentLanguage());
    }

    public string GetTranslation(LanguageManager.Language language)
    {
        switch (language)
        {
            case LanguageManager.Language.English:
                return englishText;
            case LanguageManager.Language.Spanish:
                return spanishText;
        }
        return "";
    }
}
