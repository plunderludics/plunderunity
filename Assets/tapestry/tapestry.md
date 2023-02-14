the idea here is to be able to generate a map that a character can walk around while mixing between multiple games.

maybe it might make more sense for it to be a texture that maps different games, but for the time being, i am ok with it being points in space.

a point in space represents a particular sample that is playing.

(zelda) . . . . . . . . . . .
. . . . . . . . . . (mario) .
. . . . . . . . . . . . . . .

as you move through space, samples fade in and out.

a sample is loaded by proximity to it, in case of multiples, take closest duplicate (maybe a good way of making different maps)

we can load a total of T samples
we can mix a total of M samples (currently 4)

update:
take S closest samples to player and load their states in the emulators
there's a total of S emulators running.