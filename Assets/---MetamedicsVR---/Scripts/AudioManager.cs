using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourInstance<AudioManager>
{
	private Dictionary<AudioName, AudioSource> uniqueAudios = new Dictionary<AudioName, AudioSource>();
	private Dictionary<AudioName, AudioClip> audioClips = new Dictionary<AudioName, AudioClip>();
	private Dictionary<AudioName, Dictionary<NPCManager.NPCName, AudioClip>> NPCAudios = new Dictionary<AudioName, Dictionary<NPCManager.NPCName, AudioClip>>();
	public AudioSource backgroundMusic;

	public AudioClip GetBackGroundMusic()
	{
		return backgroundMusic.clip;
	}

	public void SetBackGroundMusic(AudioName name)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			backgroundMusic.clip = clip;
			backgroundMusic.Play();
		}
	}

	public void StopBackGroundMusic()
	{
		backgroundMusic.Stop();
	}

	public float TemporalAudio(AudioName name, Transform t, Vector3 p)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			GameObject audioObject = new GameObject("TemporalAudio: " + name);
			audioObject.transform.parent = t;
			audioObject.transform.position = p;
			AudioSource audioSource = audioObject.AddComponent<AudioSource>();
			audioSource.clip = clip;
			audioSource.Play();
			AutoDestroy autoDestroy = audioObject.AddComponent<AutoDestroy>();
			autoDestroy.totalTimeToDestroy = clip.length;
			autoDestroy.StartCountDown();
			return clip.length;
		}
		return 0;
	}

	public float TemporalAudio(AudioName name, Transform t)
	{
		return TemporalAudio(name, t, t.position);
	}

	public float TemporalAudio(AudioName name, Vector3 p)
	{
		return TemporalAudio(name, null, p);
	}

	public float UniqueAudio(AudioName name, Transform t, Vector3 p)
	{
		AudioClip clip = GetAudioClip(name);
		if (clip)
		{
			AudioSource uniqueAudio = null;
			if (uniqueAudios.ContainsKey(name))
			{
				uniqueAudio = uniqueAudios[name];
			}
			if (!uniqueAudio)
			{
				GameObject audioObject = new GameObject("UniqueAudio: " + name);
				uniqueAudio = audioObject.AddComponent<AudioSource>();
				uniqueAudios[name] = uniqueAudio;
			}
			if (!uniqueAudio.isPlaying)
			{
				uniqueAudio.transform.parent = t;
				uniqueAudio.transform.position = p;
				uniqueAudio.Play();
				return clip.length;
			}
			else
			{
				return clip.length - uniqueAudio.time;
			}
		}
		return 0;
	}

	public float UniqueAudio(AudioName name, Transform t)
	{
		return UniqueAudio(name, t, t.position);
	}

	public float UniqueAudio(AudioName name, Vector3 p)
	{
		return UniqueAudio(name, null, p);
	}

	public AudioClip GetAudioClip(AudioName name)
	{
		if (name == AudioName.Null)
		{
			return null;
		}
		if (audioClips.ContainsKey(name))
		{
			return audioClips[name];
		}
		AudioClip clip = LoadAudioClip(name);
		return clip;
	}

	private AudioClip LoadAudioClip(AudioName name)
	{
		AudioClip clip = SearchAudioClip(name + "_" + LanguageManager.GetLanguageCode(LanguageManager.GetInstance().GetCurrentLanguage()));
		if (clip)
		{
			audioClips[name] = clip;
		}
		else
		{
			clip = SearchAudioClip(name.ToString());
			if (clip)
			{
				audioClips[name] = clip;
			}
		}
		return clip;
	}

	public AudioClip GetNPCClip(AudioName name, NPCManager.NPCName npc)
	{
		if (name == AudioName.Null)
		{
			return null;
		}
		if (NPCAudios.ContainsKey(name))
		{
			if (NPCAudios[name] == null)
			{
				NPCAudios[name] = new Dictionary<NPCManager.NPCName, AudioClip>();
			}
			else if (NPCAudios[name].ContainsKey(npc))
			{
				return NPCAudios[name][npc];
			}
		}
		AudioClip clip = LoadNPCClip(name, npc);
		return clip;
	}

	private AudioClip LoadNPCClip(AudioName name, NPCManager.NPCName npc)
	{
		AudioClip clip = SearchAudioClip(name + "_" + npc + "_" + LanguageManager.GetLanguageCode(LanguageManager.GetInstance().GetCurrentLanguage()));
		if (clip)
		{
			audioClips[name] = clip;
		}
		else
		{
			clip = SearchAudioClip(name + "_" + npc);
			if (clip)
			{
				audioClips[name] = clip;
			}
		}
		return clip;
	}

	private AudioClip SearchAudioClip(string name)
	{
		AudioClip clip = Resources.Load<AudioClip>("Audios/" + name);
		if(clip)
        {
			return clip;
        }
		AudioFolders[] folders = (AudioFolders[])Enum.GetValues(typeof(AudioFolders));
		foreach (AudioFolders folder in folders)
		{
			clip = Resources.Load<AudioClip>("Audios/" + folder + "/" + name);
			if (clip)
			{
				return clip;
			}
		}
		return null;
	}

	public enum AudioName
	{
		Null,
		_Abriendo_via_aerea,
		_Adrenalina_administrada,
		_Amiodarona_administrada,
		_Canula_de_Güedel_colocada,
		_Cargando_desfibrilador_a_150_julios,
		_ciclo_de_compresiones_terminado,
		_Ciclo_de_ventilación_terminado,
		_Cogiendo_vía_en_el_brazo,
		_Colocando_canula_de_güedel,
		_Colocando_parches,
		_Comprobando_consciencia,
		_Dando_descarga,
		_Descarga_completada,
		_Desfibrilador_cargado,
		_Empezando_ciclo_de_compresiones,
		_Estoy_agotandome_necesito_un_reemplazo_aquí,
		_Está_inconsciente,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7,
		_Monitorizando_con_defibrilador,
		_No_respira,
		_No_tiene_pulso,
		_Paciente_monitorizado,
		_Parches_colocados,
		_Tomando_pulso_carotídeo,
		_Ventilando_con_ambu,
		_Vía_colocada,
		_Abrir_vía_aerea,
		_Carga_de_desfibrilador,
		_Coger_vía_en_el_brazo,
		_Colocar_canula_de_güedel,
		_Colocar_parches,
		_Comprobar_consciencia,
		_Dar_descarga,
		_Empezar_ciclo_de_compresiones,
		_Está_usted_bien,
		_Inyectar_adrenalina,
		_Inyectar_Amiodarona,
		_Monitorizar_con_defibrilador,
		_Toma_de_pulso,
		_Ventilación_com_ambu
    }

	public enum AudioFolders
	{
		NpcDialogues
	}
}