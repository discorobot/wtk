# Writer's Toolkit (WTK)

![.NET Core](https://github.com/discorobot/wtk/workflows/.NET%20Core/badge.svg)

This is a minimal collection of tools designed to help writers who like to work with words the same way they work with source code.

It works best when paired with a programmer's editor and source control.

I've designed wtk to match the way I like to work. These tools are opinonated and definitely not for everyone. But iIf you're writing a novel with some kind of plain text editor and the command-line is your happy place, these may be the tools for you.




## Project Structure
I'm assuming you will create a new folder for every novel and that this folder is under some kind of source control such as git.


This folder, subfolders and files will have the following structure

```
Novel root
|   README.md
|____manuscript
|   |   section_01
|   |----chapter_01
|   |   |   01_first scene.md
|   |   |   02_second scene.md
|   |   |   synopsis.md
|   |   |   notes.md   
|   |----chapter_02
|   |   |   01_first scene in chapter 2.md
|   |   |   02_second scene in chapter 2.md
|   |   |   synopsis.md
|   |   |   notes.md
   

```
SO:
- the bulk of your novel lives under the _manuscript_ folder.
- your novel is divided into _sections_
- Each _section_ contains or more _chapters_
- Each _chapter_ contains one or more _scenes_

### format and naming conventions
- all text is written in Markdown. 
- each section folder name has the form `section_NN`or `partNN`
- each chapter folder name has the form `chapter_NN` or `chNN`. 
- each scene file has the form `NN_(anything you want).md`. Any markdown file in a chapter folder that doesnt' start with two numbers and an underscore won't be considered part of the novel when counting words or compiling
- `synopsis.md` is a special file that can exist in any chapter folder and includes a summary of what should happen in the chapter. The synopsis file can include any text, but also supports a few special codes (see below)


# WTK commands

`wtk init` sets the current directory as the top level of a wtk project. It will create a .wtk folder if one doesn't already exist

`wtk session start` Starts a writing session. This lets you track word counts and writing times for a particular period.

`wtk session stop` Stops the current writing session. A session automatically stops at the end of the day.

`wtk count` counts the total current number of words in the manuscript

`wtk count keep` counts the total number of words and writes them to the keep file (`.wtk/wc`)

`wtk count ch` counts the total word count, broken down by chapters. 

`wtk status` gives an overview of the current state of the project, based on word counts, planned word counts for each section and overall progress, including the word count for the current session.

`wtk compile`   Merges all sections and chapters into a single markdown file in the root directory.

`wtk publish`   Convert your manuscript into another format (such as .docx)

`wtk config` Set configuration values

# Publishing
You can publish to other formats (including .docx and ebook formats) with [Pandoc](https://pandoc.org/index.html). This requires a couple of configuration entries to be set up: `publish.pandoc.path` and `publish.pandoc.outputformat`


# The .wtk folder
This is generated at the top level of a wtk project and contains various system and history files. You shouldn't need to touch this. You should ensure it's committed to source control.

Files in the `.wtk` folder include
- `wc.log` A log of all word counts run with the`-k` (keep) parameter. Log entries include a date and count in the format `yyyy-MM-dd'T'HH:mm:ss xxx`

- `session` A file containing state of the current session: Time started (and the wordcount when the session started), time ended and the final wordcount in the format 
`yyyy-MM-dd'T'HH:mm:ss xxx yyyy-MM-dd'T'HH:mm:ss xxx`

- `config.json` The configuration file

### Work in progress
- [x] Implement init
- [x] Implement count
- [x] Implement count ch
- [x] Implement count keep
- [x] Implement config list
- [ ] Implement status
- [ ] Implement session
- [x] Implement compile
- [ ] Implement convert to Word (or other formats)
- [ ] Unit tests
- [ ] Integration tests
- [x] Github build
 
 