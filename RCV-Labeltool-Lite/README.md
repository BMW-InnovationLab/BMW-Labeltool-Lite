# Labeltool

Smart editor for labeling images

# Getting Started

- Install [yarn](https://yarnpkg.com) package manager and fetch the dependencies with `yarn install`
- Configure the `config.json` file for your current backend

# Build and Test

## Development

Run the application with `yarn start`. If you don't have an api, there is also a mock config available: `yarn start -c mock`

## Production Build

Build the application with `yarn build:prod`

For further documentation and instructions see the project wiki on vsts

## Docker

If you want to build it directly you can use this command in the directory of this README:

docker build --rm -f "./docker/Dockerfile" -t rcvlabeltoollite:latest "."
