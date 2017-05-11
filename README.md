# Vinder
## Stack Exchange Voting Tinder

The live, production site for Stack Exchange Voting Tinder (or Vinder as it will be noted from here-on-out) is accessible via [http://vinder.info](http://vinder.info). Anyone may login with their Stack Exchange account credentials and participate in voting. Do note that this is a beta project, and as a result there may be issues ranging from minor to major in severity that are not resolved. As an open-source, free-ware project this software is still in a constant state of development.

Anyone may participate in the GitHub repository located at [https://github.com/EBrown8534/Vinder](https://github.com/EBrown8534/Vinder), all contributions will be reviewed before being accepted, but the timeline for this will hopefully be very quick. If users wish to get code suggestions reviewed by additional parties, then we recommend that they visit [Code Review Stack Exchange](https://codereview.stackexchange.com/) and post a question. This may help expedite the process for integrating merges and changes as well.

## About

This project, originally proposed by [Simon Forsberg from Code Review Stack Exchange](https://codereview.stackexchange.com/users/31562/simon-forsberg) and implemented by [EBrown from the same community](https://codereview.stackexchange.com/users/73844/ebrown), was initially designed to provide a voting experience somewhat separate from the traditional Stack Exchange voting experience that exists. It is designed to provide a 'Tinder-esque' style of voting, where users are presented with either a question or answer, and minimal tertiary information (such as author, reputation for the author, even the existing question/answer score) to create an unbiased vote for or against content, instead of users.

The purpose of this project is to eliminate the 'voting bias' that inevitably exists on communities such as this, and encourage users to make quicker, simpler decisions when voting for content. By removing a significant portion of the supporting, tertiary information, we hope to create an environment where users are not voted for, but the content is.

The only caveat to this style of voting technique, is that Stack Exchange requires CC-BY-SA attribution for all content, meaning we cannot *simply* strip the author out entirely. Instead, we must display the author (as expected), however we may make it *somewhat* visually obscure.

## Dependencies

This project depends on the [Evbpc.Framework](https://github.com/EBrown8534/Framework/tree/master/Evbpc.Framework) project from the [Framework repository by EBrown8534](https://github.com/EBrown8534/Framework).

This project also depends on version 4.5 of the .NET Framework.