# R7.Webmaster

[![BCH compliance](https://bettercodehub.com/edge/badge/roman-yagodin/R7.Webmaster)](https://bettercodehub.com/)
[![Build Status](https://travis-ci.org/roman-yagodin/R7.Webmaster.svg?branch=master)](https://travis-ci.org/roman-yagodin/R7.Webmaster)

*R7.Webmaster* project's main goal is to increase webmasters everyday productivity by providing a set
of useful and (hopefully) ergonomic desktop tools. And I also think that these tools should be
cross-platform (1) and extensible (2).

![Screenshot](https://raw.githubusercontent.com/roman-yagodin/R7.Webmaster/master/images/webmaster_textcleaner.png "R7.Webmaster main application with TextCleaner addin")

## Functionality

*R7.Webmaster* consists of main host application, which serves as a platform for extension modules (addins)
with end-user functionality. In its current (development) state *R7.Webmaster* include following addins by default:

1. Text cleaner - to cleanup text and convert it to HTML;
2. Case changer - to easily change text case to 5 variants;
3. Password generator - to generate passwords or GUIDs;
4. Ratio calculator - to easily perform width / height ratio calculations. 

More addins are planned in the near future. Those are: character map, code templates (T4-based), htmltidy / csstidy frontends,
embedded file manager, SEO tools, color / palette tools. 

## Extensibility

Each addin is [Mono addin](http://www.mono-project.com/archived/monoaddins/), so it will be relatively easy 
to average developer to create it's own addin using preferred .NET / Mono programming language 
and then plug it to the main *R7.Webmaster* application.

## GUI

*R7.Webmaster* main application is a GTK# 2 application, so it could run on all Mono-enabled desktop platforms (Linux, Windows and Mac) 
with little effort (Linux and Windows versions are planned for the first release). Every addin is mostly GTK# widget.

I'll try to implement some kind of MVP-VM approach in the main application and default addins architecture, 
but in the current state of development it's more like ugly Document-View - but I'm working on it.

## Configuration

Main application and default adding use Nini configuration library to access configs. Currently configs are stored
in .NET configuration XML-based format. 

Third-party addin could implement it's own configuraton storage, or define child class of ConfigBase in R7.Webmaster.Core 
namespace. ConfigBase class currently provides automatic deployment of the config files to user profiles 
and platform-awareness by returning only a set of settings for the current platform (OS).

## Command-line

Command-line and scripting support is also planned, and MVP approach and Nini library was choosen also because of that.
