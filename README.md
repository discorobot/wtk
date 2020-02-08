# Writer's Toolkit (WTK)

This is a collection of tools designed to help writers who like to work with words the same way they work with source code.

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
- each section folder name has the form `section_NN`
- each chapter folder name has the form `chapter_NN`
- each scene file has the form `NN_(anything you want).md`. Any markdown file in a chapter folder that doesnt' start with two numbers and an underscore won't be considered part of the novel when counting words or compiling
- `synopsis.md` is a special file that can exist in any chapter folder and includes a summary of what should happen in the chapter. The synopsis file can include any text, but also supports a few special codes (see below)


# WTK command line

`wtk init` sets the current directory as the top level of a wtk project. It will create a .wtk folder if one doesn't already exist

`wtk count` counts the total current number of words in the manuscript

`wtk count -c` counts the total word count, broken down by chapters. 

`wtk status` gives an overview of the current state of the project, based on word counts, planned word counts for each section and overall progress.

`wtk compile`   Merges all sections and chapters into a single markdown file in the root directory.

# The .wtk folder
This is generated at the top level of a wtk project and contains various system and history files. You shouldn't need to touch this. You should ensure it's committed to source control.