import { version } from './version'

export const environment = {
  version: version,
  production: true,
  configurationFile: 'config/config.json',
  services: {
    mock: false,
  },
}
