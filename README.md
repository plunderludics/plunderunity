# plunderunity
a unity project for managing the plundering of games, mostly display right now, but hopefully more fun stuff in the future!

at this time this project serves mostly to consolidate ideas from Touch Designer into a thing that is more feasable to package into a zip file that people could then download and play.

currently the project has the following osc channels:

`/show/[n]` shows the n'th screen from the available screens

`/hide/[n]` hides the n'th screen from the available screens

`/solo/[n]` shows the n'th screen from the available screens and hides every other screen


## dependencies


[OscJack](https://github.com/keijiro/OscJack) for receiving osc signals

[uWindowCapture](https://openupm.com/packages/com.hecomi.uwindowcapture/) for capturing background windows while the unity game plays.

