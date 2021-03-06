﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Made by      : Abia Herlianto
// Description  : Generates notes based on song data, depending on the current
//                time of the song as well as other calculations to create a
//                more efficient method compared to spawning all of them at
//                once

/// <summary>
/// A component to generate the song notes over time.
/// </summary>
public class NoteGenerator : MonoBehaviour
{
    public GameObject fireNote;
    public GameObject airNote;
    public GameObject waterNote;
    public GameObject earthNote;
    public GameObject empoweredFireNote;
    public GameObject empoweredAirNote;
    public GameObject empoweredWaterNote;
    public GameObject empoweredEarthNote;

    public GameObject bottomLane;
    public GameObject middleLane;
    public GameObject topLane;

    public float noteSpeed = 0.0f;
    public float hitOffset = 0.075f;
    public float greatHitOffset = 0.1f;
    public float perfectHitOffset = 0.25f;
    public bool isPaused;

    private bool isInit = false;
    private SongParser.Metadata songData;
    private float songTimer = 0.0f;
    private float barTime = 0.0f;
    private float barExecutedTime = 0.0f;
    private GameObject player;
    private AudioSource audioSource;
    private SongParser.NoteData noteData;
    private float distance;
    private float originalDistance = 10.0f;
    private int barCount = 0;

    private float offsetSongTimer = 0.0f;
    private bool isPlacingBar = false;

    // Start is called before the first frame update
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = player.GetComponent<AudioSource>();

        greatHitOffset = hitOffset - greatHitOffset;
        perfectHitOffset = hitOffset - perfectHitOffset;

        isPaused = false;
    }

    // Update is called once per frame
    private void Update()
    {
        // if done initialising the rest of the world
        // and haven't gone through all bars of the song yet
        if (isInit && barCount < noteData.bars.Count && !isPaused)
        {
            // calculate time offset using s = d /t equation
            // (t = d / s)
            distance = originalDistance;
            // float _timeOffset = distance * noteSpeed;
            float _timeOffset = distance / (noteSpeed / Time.deltaTime);

            // get current time through song
            songTimer = audioSource.time;

            //if (barCount == 0)
            //{
            //    barExecutedTime = songTimer - _timeOffset;
            //}

            //// if current song time - time offset is greater than
            //// time taken for all executed bars so far
            //// spawn the next bar's notes
            //if (songTimer - _timeOffset >= (barExecutedTime - barTime))
            //{
            //    if (!isPlacingBar)
            //    {
            //        StartCoroutine(PlaceBar(noteData.bars[barCount++]));

            //        barExecutedTime += barTime;
            //    }

            //    Debug.Log(barCount);
            //}

            offsetSongTimer += Time.deltaTime;

            if (barCount == 0)
            {
                offsetSongTimer = songTimer - _timeOffset;
                Debug.Log("Start Offset: " + offsetSongTimer);
            }

            // if current song time - time offset is greater than
            // time taken for all executed bars so far
            // spawn the next bar's notes
            if (songTimer - _timeOffset >= offsetSongTimer)
            {
                if (!isPlacingBar)
                {
                    StartCoroutine(PlaceBar(noteData.bars[barCount++]));

                    // barExecutedTime += barTime;
                }

                Debug.Log("Bar Count: " + barCount);
            }

            Debug.Log("Offset Song Timer: " + offsetSongTimer);

            // Debug.Log(barExecutedTime);
        }
    }

    // go through all notes in a bar
    // creates an instance of note prefab
    // depending on which note is meant to be spawned
    private IEnumerator PlaceBar(List<SongParser.Notes> bar)
    {
        isPlacingBar = true;

        for (int i = 0; i < bar.Count; i++)
        {
            //if (IsThereNote(bar[i].bottom))
            //{
            //    GameObject _obj = (GameObject)Instantiate(GetNotePrefab(bar[i].bottom, true), new Vector3(bottomLane.transform.position.x + distance, bottomLane.transform.position.y, bottomLane.transform.position.z - 0.3f), Quaternion.identity);
            //    GameData.currentSongStats.notesCounter++;
            //}
            if (bar[i].middle != 0)
            {
                //GameObject _obj = (GameObject)Instantiate(GetNotePrefab(bar[i].middle, false), new Vector3(middleLane.transform.position.x + distance, middleLane.transform.position.y, middleLane.transform.position.z - 0.3f), Quaternion.identity);
                GameObject _obj = ObjectPooler.instance.GetObject((int)bar[i].middle - 1);
                _obj.transform.position = new Vector3(middleLane.transform.position.x + distance, middleLane.transform.position.y, middleLane.transform.position.z - 0.3f);
                _obj.transform.rotation = Quaternion.identity;
                _obj.SetActive(true);
                GameData.currentSongStats.notesCounter++;
            }
            //if (bar[i].top != 0)
            //{
            //    GameObject _obj = (GameObject)Instantiate(GetNotePrefab(bar[i].top, true), new Vector3(topLane.transform.position.x + distance, topLane.transform.position.y, topLane.transform.position.z - 0.3f), Quaternion.identity);
            //    GameData.currentSongStats.notesCounter++;
            //}

            yield return new WaitForSeconds((barTime / bar.Count) - Time.deltaTime);
        }

        isPlacingBar = false;
    }

    // iniatialises variables and sets arrow speed
    public void InitNotes(SongParser.Metadata newSongData)
    {
        songData = newSongData;
        isInit = true;
        // Debug.Log("isInit set to: " + isInit);

        // estimate how many seconds a single 'bar' will be in the
        // song using the bpm in song data
        // Debug.Log("BPM: " + songData.bpm);
        barTime = (60.0f / songData.bpm) * 4.0f;
        Debug.Log("barTime: " + barTime);

        distance = originalDistance;
        // Debug.Log("distance: " + distance);

        // TO-DO: Integrate note speed into song data
        // how fast the arrow will be going
        if (noteSpeed <= 0.0f)
        {
            noteSpeed = 0.05f;
        }
        // noteSpeed = 0.009f; // TEMPORARY MAGIC NUMBER
        noteData = songData.noteData;
    }

    // returns appropriate note prefab
    private GameObject GetNotePrefab(SongParser.NoteType noteType, bool isEmpowered)
    {
        GameObject _obj = null;

        switch (noteType)
        {
            case SongParser.NoteType.Fire:
                if (isEmpowered)
                {
                    _obj = empoweredFireNote;
                }
                else
                {
                    _obj = fireNote;
                }
                break;
            case SongParser.NoteType.Air:
                if (isEmpowered)
                {
                    _obj = empoweredAirNote;
                }
                else
                {
                    _obj = airNote;
                }
                break;
            case SongParser.NoteType.Water:
                if (isEmpowered)
                {
                    _obj = empoweredWaterNote;
                }
                else
                {
                    _obj = waterNote;
                }
                break;
            case SongParser.NoteType.Earth:
                if (isEmpowered)
                {
                    _obj = empoweredEarthNote;
                }
                else
                {
                    _obj = earthNote;
                }
                break;
            default:
                _obj = fireNote;
                break;
        }

        return _obj;
    }

    // checks if the note type exists
    private bool IsThereNote(SongParser.NoteType note)
    {
        switch (note)
        {
            case SongParser.NoteType.Fire:
            case SongParser.NoteType.Air:
            case SongParser.NoteType.Water:
            case SongParser.NoteType.Earth:
                // Debug.Log("Return true");
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Pauses notes depending on _isPaused parameter.
    /// </summary>
    /// <param name="_isPaused"></param>
    public void PauseNotes(bool _isPaused)
    {
        isPaused = _isPaused;
    }
}
