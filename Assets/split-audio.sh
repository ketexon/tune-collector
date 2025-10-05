#!/bin/bash

FILE_NAME=$1
OUT_DIR=$2
NUM_SEMITONES=$3
START_OCTAVE=$4
START_SEMITONE=$5
PADDING=0.1

if [ -z "$FILE_NAME" ] || [ -z "$NUM_SEMITONES" ] || [ -z "$START_OCTAVE" ] || [ -z "$START_SEMITONE" ]; then
	echo "Usage: $0 <file_name> <num_semitones> <start_octave> <start_semitone>"
	exit 1
fi

mkdir -p "$OUT_DIR"

TOTALDURATION=$(ffprobe \
	-v error \
	-show_entries format=duration \
	-of default=noprint_wrappers=1:nokey=1 \
	"$FILE_NAME")

function semi_to_name() {
	case $1 in
		0) echo "C" ;;
		1) echo "C#" ;;
		2) echo "D" ;;
		3) echo "D#" ;;
		4) echo "E" ;;
		5) echo "F" ;;
		6) echo "F#" ;;
		7) echo "G" ;;
		8) echo "G#" ;;
		9) echo "A" ;;
		10) echo "A#" ;;
		11) echo "B" ;;
	esac
}

# use awk not BC to get leading 0
NOTE_DURATION=$(awk '{ printf "%f", $1 / $2 - 2 * $3}' <<< "$TOTALDURATION $NUM_SEMITONES $PADDING")

for (( i=0; i<NUM_SEMITONES; i++ )); do
	OCTAVE=$((START_OCTAVE + (START_SEMITONE + i) / 12))
	SEMITONE=$(( (START_SEMITONE + i) % 12 ))
	NOTE_NAME=$(semi_to_name $SEMITONE)

	START_TIME=$(awk '{ printf "%f", $1 * $2 / $3 + $5}' <<< "$i $TOTALDURATION $NUM_SEMITONES $PADDING")

	OUTPUT_FILE="${OUT_DIR}/${FILE_NAME%.*}${i}_${NOTE_NAME}${OCTAVE}.mp3"
	echo "Generating $OUTPUT_FILE"
	ffmpeg -y \
		-i "$FILE_NAME" \
		-ss "$START_TIME" \
		-t "$NOTE_DURATION" \
		-ac 2 \
		-ar 44100 \
		"$OUTPUT_FILE"
done