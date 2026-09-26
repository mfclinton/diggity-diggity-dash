# Diggity Diggity Diggity (Dash)

You race other moles for the championship cup, digging your own tunnels through dirt that breaks apart as you go.

- Play: [itch.io](https://unitedfailures.itch.io/diggity-diggity-diggity-dash)
- Made: April 2025 for Ludum Dare 57
- Team: [@mfclinton](https://github.com/mfclinton) (programming), [@DavidKoleczek](https://github.com/DavidKoleczek) (programming)
- Engine: Unity, C#

This is an export of a private repo with only the code we wrote. Art, audio, the Unity project files, and third party plugins aren't included. The history is squashed into one commit.

## What I built

- I built the diggable terrain with marching squares. The dirt is a grid of density values split into chunks, and digging only rebuilds the chunks it touches. The starting dirt can come from Perlin noise or be drawn as an image.
- The terrain's outline doubles as its collision. The edges get stitched into closed shapes for each chunk, and points where the outline barely changes direction are dropped.
- Digging uses a round brush in front of the mole that digs hardest in the middle, so tunnels come out smooth instead of blocky.
- AI racers plan their route with A*, where going through dirt costs more than going through open tunnel. They re-plan every couple of seconds as the tunnels change, and they control their moles through the same input setup as the player.
