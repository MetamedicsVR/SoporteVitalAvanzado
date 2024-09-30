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
		_Abriendo_via_aerea_Jesús_,
		_Abriendo_via_aerea_Rubén_,
		_Adrenalina_administrada_Carla_,
		_Adrenalina_administrada_David_,
		_Adrenalina_administrada_Jesús_,
		_Adrenalina_administrada_Rubén_,
		_Amiodarona_administrada_Carla_,
		_Amiodarona_administrada_David_,
		_Amiodarona_administrada_Jesús_,
		_Amiodarona_administrada_Rubén_,
		_Canula_de_Güedel_colocada_Carla_,
		_Canula_de_Güedel_colocada_David_,
		_Canula_de_Güedel_colocada_Jesús_,
		_Canula_de_Güedel_colocada_Rubén_,
		_Cargando_desfibrilador_a_150_julios_Carla_,
		_Cargando_desfibrilador_a_150_julios_David_,
		_Cargando_desfibrilador_a_150_julios_Jesús_,
		_Cargando_desfibrilador_a_150_julios_Rubén_,
		_ciclo_de_compresiones_terminado_Carla_,
		_ciclo_de_compresiones_terminado_David_,
		_ciclo_de_compresiones_terminado_Jesús_,
		_ciclo_de_compresiones_terminado_Rubén_,
		_Ciclo_de_ventilación_terminado_Carla_,
		_Ciclo_de_ventilación_terminado_David_,
		_Ciclo_de_ventilación_terminado_Jesús_,
		_Ciclo_de_ventilación_terminado_Rubén_,
		_Cogiendo_vía_en_el_brazo_Carla_,
		_Cogiendo_vía_en_el_brazo_David_,
		_Cogiendo_vía_en_el_brazo_Jesús_,
		_Cogiendo_vía_en_el_brazo_Rubén_,
		_Colocando_canula_de_güedel_Carla_,
		_Colocando_canula_de_güedel_David_,
		_Colocando_canula_de_güedel_Jesús_,
		_Colocando_canula_de_güedel_Rubén_,
		_Colocando_parches_Carla_,
		_Colocando_parches_David_,
		_Colocando_parches_Jesús_,
		_Colocando_parches_Rubén_,
		_Comprobando_consciencia_Carla_,
		_Comprobando_consciencia_David_,
		_Comprobando_consciencia_Jesús_,
		_Comprobando_consciencia_Rubén_,
		_Dando_descarga_Carla_,
		_Dando_descarga_David_,
		_Dando_descarga_Jesús_,
		_Dando_descarga_Rubén_,
		_Descarga_completada_Carla_,
		_Descarga_completada_David_,
		_Descarga_completada_Jesús_,
		_Descarga_completada_Rubén_,
		_Desfibrilador_cargado_Carla_,
		_Desfibrilador_cargado_David_,
		_Desfibrilador_cargado_Jesús_,
		_Desfibrilador_cargado_Rubén_,
		_Empezando_ciclo_de_compresiones_Carla_,
		_Empezando_ciclo_de_compresiones_David_,
		_Empezando_ciclo_de_compresiones_Jesús_,
		_Empezando_ciclo_de_compresiones_Rubén_,
		__Estoy_agotandome_necesito_un_reemplazo_aquí_Carla_,
		_Estoy_agotandome_necesito_un_reemplazo_aquí_David_,
		_Estoy_agotandome_necesito_un_reemplazo_aquí_Jesús_,
		_Estoy_agotandome_necesito_un_reemplazo_aquí_Rubén_,
		_Está_inconsciente_Carla_,
		_Está_inconsciente_David_,
		_Está_inconsciente_Jesús_,
		_Está_inconsciente_Rubén_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Carla_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_David_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Jesús_,
		_Inyectando_1_miligramo_de_adrenalina_Intravenosa_Rubén_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Carla_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_David_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Jesús_,
		_Inyectando_300_miligramos_amiodarona_en_20_mililit_c1cce3a7_Rubén_,
		_Monitorizando_con_defibrilador_Carla_,
		_Monitorizando_con_defibrilador_David_,
		_Monitorizando_con_defibrilador_Jesús_,
		_Monitorizando_con_defibrilador_Rubén_,
		_No_respira_Carla_,
		_No_respira_David_,
		_No_respira_Jesús_,
		_No_respira_Rubén_,
		_No_tiene_pulso_Carla_,
		_No_tiene_pulso_David_,
		_No_tiene_pulso_Jesús_,
		_No_tiene_pulso_Rubén_,
		_Paciente_monitorizado_Carla_,
		_Paciente_monitorizado_David_,
		_Paciente_monitorizado_Jesús_,
		_Paciente_monitorizado_Rubén_,
		_Parches_colocados_Carla_,
		_Parches_colocados_David_,
		_Parches_colocados_Jesús_,
		_Parches_colocados_Rubén_,
		_Tomando_pulso_carotídeo_Carla_,
		_Tomando_pulso_carotídeo_David_,
		_Tomando_pulso_carotídeo_Jesús_,
		_Tomando_pulso_carotídeo_Rubén_,
		_Ventilando_con_ambu_Carla_,
		_Ventilando_con_ambu_David_,
		_Ventilando_con_ambu_Jesús_,
		_Ventilando_con_ambu_Rubén_1,
		_Vía_colocada_Carla_,
		_Vía_colocada_David_,
		_Vía_colocada_Jesús_,
		_Vía_colocada_Rubén_,
		_Abrir_vía_aerea_Carla_,
		_Abrir_vía_aerea_David_,
		_Abrir_vía_aerea_Jesús_,
		_Abrir_vía_aerea_Rubén_,
		_Carga_de_desfibrilador_Carla_,
		_Carga_de_desfibrilador_David_,
		_Carga_de_desfibrilador_Jesús_,
		_Carga_de_desfibrilador_Rubén_,
		_Coger_vía_en_el_brazo_Carla_,
		_Coger_vía_en_el_brazo_David_,
		_Coger_vía_en_el_brazo_Jesús_,
		_Coger_vía_en_el_brazo_Rubén_,
		_Colocar_canula_de_güedel_Carla_,
		_Colocar_canula_de_güedel_David_,
		_Colocar_canula_de_güedel_Jesús_,
		_Colocar_canula_de_güedel_Rubén_,
		_Colocar_parches_Carla_,
		_Colocar_parches_David_,
		_Colocar_parches_Jesús_,
		_Colocar_parches_Rubén_,
		_Comprobar_consciencia_Carla_,
		_Comprobar_consciencia_David_,
		_Comprobar_consciencia_Jesús_,
		_Comprobar_consciencia_Rubén_,
		_Dar_descarga_Carla_,
		_Dar_descarga_David_,
		_Dar_descarga_Jesús_,
		_Dar_descarga_Rubén_,
		_Empezar_ciclo_de_compresiones_Carla_,
		_Empezar_ciclo_de_compresiones_David_,
		_Empezar_ciclo_de_compresiones_Jesús_,
		_Empezar_ciclo_de_compresiones_Rubén_,
		_Está_usted_bien_Carla_,
		_Está_usted_bien_David_,
		_Está_usted_bien_Jesús_,
		_Está_usted_bien_Rubén_,
		_Inyectar_adrenalina_Carla_,
		_Inyectar_adrenalina_David_,
		_Inyectar_adrenalina_Jesús_,
		_Inyectar_adrenalina_Rubén_,
		_Inyectar_Amiodarona_Carla_,
		_Inyectar_Amiodarona_David_,
		_Inyectar_Amiodarona_Jesús_,
		_Inyectar_Amiodarona_Rubén_,
		_Monitorizar_con_defibrilador_Carla_,
		_Monitorizar_con_defibrilador_David_,
		_Monitorizar_con_defibrilador_Jesús_,
		_Monitorizar_con_defibrilador_Rubén_,
		_Toma_de_pulso_Carla_,
		_Toma_de_pulso_David_,
		_Toma_de_pulso_Jesús_,
		_Toma_de_pulso_Rubén_,
		_Ventilación_com_ambu_Carla_,
		_Ventilación_com_ambu_David_,
		_Ventilación_com_ambu_Jesús_,
		_Ventilación_com_ambu_Rubén_,
		Null,
        _No_tiene_pulso
    }

	public enum AudioFolders
	{
		NpcDialogues
	}
}