import argparse
import pathlib
from sys import exit
import re

parser = argparse.ArgumentParser(description="Generate pattern notes.")

parser.add_argument("-f", "--file", type=str, help="Path to the .asset file")
parser.add_argument("notes", nargs=argparse.REMAINDER, help="List of notes to add. Syntax: C4:1/4 R:1/4 E4:1/2...")

args = parser.parse_args()

notes = args.notes

if not notes:
    parser.print_help()
    exit(1)

notes2 = []

for note in notes:
    notes2.extend(note.split())

notes = notes2

if args.file:
    asset_path = pathlib.Path(args.file)
    if asset_path.exists():
        overwrite = input("Warning: File already exists. Overwrite? [Y/N] ")
        if overwrite.lower() != "y":
            print("Aborting.")
            exit(0)

note_re = re.compile(r"^([A-G][#b]?\d|R):(\d+)/(\d+)$")
pitch_re = re.compile(r"^([A-G][#b]?)(\d)$")

semitone_map = {
    "C": 0,
    "C#": 1, "Db": 1,
    "D": 2,
    "D#": 3, "Eb": 3,
    "E": 4,
    "F": 5,
    "F#": 6, "Gb": 6,
    "G": 7,
    "G#": 8, "Ab": 8,
    "A": 9,
    "A#": 10, "Bb": 10,
    "B": 11
}

min_semitone = semitone_map["G"] + (4 * 12)
max_semitone = semitone_map["G"] + (6 * 12)
# rest is 0, so G4 = 1
start_semitone = min_semitone - 1 # G4 = 1

out = []

total_duration = 0

for note in notes:
    match = note_re.match(note)
    if not match:
        print(f"Error: Note '{note}' is not in the correct format. Expected format: C4:1/4")
        exit(1)

    pitch, numerator, denominator = match.groups()
    if pitch == "R":
        pitch_value = 0
    else:
        pitch_match = pitch_re.match(pitch)
        assert(pitch_match)
        pitch, octave = pitch_match.groups()

        if pitch not in semitone_map:
            print(f"Error: Pitch '{pitch}' is not valid. Use pitches like C, C#, D, D#, E, F, F#, G, G#, A, A#, B.")
            exit(1)

        semitone = semitone_map[pitch] + (int(octave) * 12)
        pitch_value = semitone - start_semitone
        if semitone < min_semitone or semitone > max_semitone:
            print(f"Error: Pitch '{pitch}{octave}' is out of range. Supported range is G4 to G6.")
            exit(1)

    total_duration += int(numerator) / int(denominator)

    out.append(f"""  - Duration: {numerator}\n    Division: {denominator}\n    Pitch: {pitch_value}\n    Element: {{fileID: 0}}""")

if total_duration > 1:
    print(f"Error: Total duration exceeds 1 measures. Current total: {total_duration} measures.")
    exit(1)



yaml_notes_out = "\n".join(out)

if not args.file:
    print(yaml_notes_out)
    exit(0)

asset_path = pathlib.Path(args.file)

yaml_out = f"""%YAML 1.1
%TAG !u! tag:unity3d.com,2011:
--- !u!114 &11400000
MonoBehaviour:
  m_ObjectHideFlags: 0
  m_CorrespondingSourceObject: {{fileID: 0}}
  m_PrefabInstance: {{fileID: 0}}
  m_PrefabAsset: {{fileID: 0}}
  m_GameObject: {{fileID: 0}}
  m_Enabled: 1
  m_EditorHideFlags: 0
  m_Script: {{fileID: 11500000, guid: aa4e47b2e0583854f8f3a56b00294da7, type: 3}}
  m_Name: {asset_path.stem}
  m_EditorClassIdentifier: Assembly-CSharp::Pattern
  Notes:
{yaml_notes_out}
"""

asset_path.write_text(yaml_out)