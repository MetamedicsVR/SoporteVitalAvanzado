using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviourInstance<AudioManager>
{
	private Dictionary<AudioName, AudioSource> uniqueAudios = new Dictionary<AudioName, AudioSource>();
	private Dictionary<AudioName, AudioClip> audioClips = new Dictionary<AudioName, AudioClip>();
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
		if (audioClips.ContainsKey(name))
		{
			return audioClips[name];
		}
		AudioClip clip = LoadAudioClip(name);
		return clip;
	}

	public AudioClip LoadAudioClip(AudioName name)
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
		_Abriendo_via_aerea_Carla_,
		_Abriendo_via_aerea_David_,
		_Abriendo_via_aerea_Jes�s_,
		_Abriendo_via_aerea_Rub�n_,
		_Adrenalina_administrada_Carla_,
		_Adrenalina_administrada_David_,
		_Adrenalina_administrada_Jes�s_,
		_Adrenalina_administrada_Rub�n_,
		_Amiodarona_administrada_Carla_,
		_Amiodarona_administrada_David_,
		_Amiodarona_administrada_Jes�s_,
		_Amiodarona_administrada_Rub�n_,
		_Canula_de_G�edel_colocada_Carla_,
		_Canula_de_G�edel_colocada_David_,
		_Canula_de_G�edel_colocada_Jes�s_,
		_Canula_de_G�edel_colocada_Rub�n_,
		_Cargando_desfibrilador_a_150_julios_Carla_,
		_Cargando_desfibrilador_a_150_julios_David_,
		_Cargando_desfibrilador_a_150_julios_Jes�s_,
		_Cargando_desfibrilador_a_150_julios_Rub�n_,
		_ciclo_de_compresiones_terminado_Carla_,
		_ciclo_de_compresiones_terminado_David_,
		_ciclo_de_compresiones_terminado_Jes�s_,
		_ciclo_de_compresiones_terminado_Rub�n_,
		_Ciclo_de_ventilaci�n_terminado_Carla_,
		_Ciclo_de_ventilaci�n_terminado_David_,
		_Ciclo_de_ventilaci�n_terminado_Jes�s_,
		_Ciclo_de_ventilaci�n_terminado_Rub�n_,
		_Cogiendo_v�a_en_el_brazo_Carla_,
		_Cogiendo_v�a_en_el_brazo_David_,
		_Cogiendo_v�a_en_el_brazo_Jes�s_,
		_Cogiendo_v�a_en_el_brazo_Rub�n_,
		_Colocando_canula_de_g�edel_Carla_,
		_Colocando_canula_de_g�edel_David_,
		_Colocando_canula_de_g�edel_Jes�s_,
		_Colocando_canula_de_g�edel_Rub�n_,
		_Colocando_parches_Carla_,
		_Colocando_parches_David_,
		_Colocando_parches_Jes�s_,
		_Colocando_parches_Rub�n_,
		_Comprobando_consciencia_Carla_,
		_Comprobando_consciencia_David_,
		_Comprobando_consciencia_Jes�s_,
		_Comprobando_consciencia_Rub�n_,
		_Dando_descarga_Carla_,
		_Dando_descarga_David_,
		_Dando_descarga_Jes�s_,
		_Dando_descarga_Rub�n_,
		_Descarga_completada_Carla_,
		_Descarga_completada_David_,
		_Descarga_completada_Jes�s_,
		_Descarga_completada_Rub�n_,
		_Desfibrilador_cargado_Carla_,
		_Desfibrilador_cargado_David_,
		_Desfibrilador_cargado_Jes�s_,
		_Desfibrilador_cargado_Rub�n_,
		_Empezando_ciclo_de_compresiones_Carla_,
		_Empezando_ciclo_de_compresiones_David_,
		_Empezando_ciclo_de_compresiones_Jes�s_,
		_Empezando_ciclo_de_compresiones_Rub�n_,
		__Estoy_agotandome_necesito_un_reemplazo_aqu�_Carla_,
		_Estoy_agotandome_necesito_un_reemplazo_aqu�_David_,
		_Estoy_agotandome_necesito_un_reemplazo_aqu�_Jes�s_,
		_Estoy_agotandome_necesito_un_reemplazo_aqu�_Rub�n_,
		_Est�_inconsciente_Carla_,
		_Est�_inconsciente_David_,
		_Est�_inconsciente_Jes�s_,
		_Est�_inconsciente_Rub�n_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Carla_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_David_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Jes�s_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Rub�n_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Carla_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_David_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Jes�s_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Rub�n_,
		_Monitorizando_con_defibrilador_Carla_,
		_Monitorizando_con_defibrilador_David_,
		_Monitorizando_con_defibrilador_Jes�s_,
		_Monitorizando_con_defibrilador_Rub�n_,
		_No_respira_Carla_,
		_No_respira_David_,
		_No_respira_Jes�s_,
		_No_respira_Rub�n_,
		_No_tiene_pulso_Carla_,
		_No_tiene_pulso_David_,
		_No_tiene_pulso_Jes�s_,
		_No_tiene_pulso_Rub�n_,
		_Paciente_monitorizado_Carla_,
		_Paciente_monitorizado_David_,
		_Paciente_monitorizado_Jes�s_,
		_Paciente_monitorizado_Rub�n_,
		_Parches_colocados_Carla_,
		_Parches_colocados_David_,
		_Parches_colocados_Jes�s_,
		_Parches_colocados_Rub�n_,
		_Tomando_pulso_carot�deo_Carla_,
		_Tomando_pulso_carot�deo_David_,
		_Tomando_pulso_carot�deo_Jes�s_,
		_Tomando_pulso_carot�deo_Rub�n_,
		_Ventilando_con_ambu_Carla_,
		_Ventilando_con_ambu_David_,
		_Ventilando_con_ambu_Jes�s_,
		_Ventilando_con_ambu_Rub�n_1,
		_V�a_colocada_Carla_,
		_V�a_colocada_David_,
		_V�a_colocada_Jes�s_,
		_V�a_colocada_Rub�n_,
		_Abrir_v�a_aerea_Carla_,
		_Abrir_v�a_aerea_David_,
		_Abrir_v�a_aerea_Jes�s_,
		_Abrir_v�a_aerea_Rub�n_,
		_Carga_de_desfibrilador_Carla_,
		_Carga_de_desfibrilador_David_,
		_Carga_de_desfibrilador_Jes�s_,
		_Carga_de_desfibrilador_Rub�n_,
		_Coger_v�a_en_el_brazo_Carla_,
		_Coger_v�a_en_el_brazo_David_,
		_Coger_v�a_en_el_brazo_Jes�s_,
		_Coger_v�a_en_el_brazo_Rub�n_,
		_Colocar_canula_de_g�edel_Carla_,
		_Colocar_canula_de_g�edel_David_,
		_Colocar_canula_de_g�edel_Jes�s_,
		_Colocar_canula_de_g�edel_Rub�n_,
		_Colocar_parches_Carla_,
		_Colocar_parches_David_,
		_Colocar_parches_Jes�s_,
		_Colocar_parches_Rub�n_,
		_Comprobar_consciencia_Carla_,
		_Comprobar_consciencia_David_,
		_Comprobar_consciencia_Jes�s_,
		_Comprobar_consciencia_Rub�n_,
		_Dar_descarga_Carla_,
		_Dar_descarga_David_,
		_Dar_descarga_Jes�s_,
		_Dar_descarga_Rub�n_,
		_Empezar_ciclo_de_compresiones_Carla_,
		_Empezar_ciclo_de_compresiones_David_,
		_Empezar_ciclo_de_compresiones_Jes�s_,
		_Empezar_ciclo_de_compresiones_Rub�n_,
		_Est�_usted_bien_Carla_,
		_Est�_usted_bien_David_,
		_Est�_usted_bien_Jes�s_,
		_Est�_usted_bien_Rub�n_,
		_Inyectar_adrenalina_Carla_,
		_Inyectar_adrenalina_David_,
		_Inyectar_adrenalina_Jes�s_,
		_Inyectar_adrenalina_Rub�n_,
		_Inyectar_Amiodarona_Carla_,
		_Inyectar_Amiodarona_David_,
		_Inyectar_Amiodarona_Jes�s_,
		_Inyectar_Amiodarona_Rub�n_,
		_Monitorizar_con_defibrilador_Carla_,
		_Monitorizar_con_defibrilador_David_,
		_Monitorizar_con_defibrilador_Jes�s_,
		_Monitorizar_con_defibrilador_Rub�n_,
		_Toma_de_pulso_Carla_,
		_Toma_de_pulso_David_,
		_Toma_de_pulso_Jes�s_,
		_Toma_de_pulso_Rub�n_,
		_Ventilaci�n_com_ambu_Carla_,
		_Ventilaci�n_com_ambu_David_,
		_Ventilaci�n_com_ambu_Jes�s_,
		_Ventilaci�n_com_ambu_Rub�n_,
		Null,
        _No_tiene_pulso
    }

	public enum AudioFolders
	{
		NpcDialogues
	}
}