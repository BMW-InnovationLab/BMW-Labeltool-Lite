import { RcvLabelMode } from '@rcv/domain'

export const objectDetectionPath = 'object-detection'

export const labelModeRoutes = {
  objectDetection: objectDetectionPath,
}

export function labelModePath(mode: RcvLabelMode): string {
  return objectDetectionPath
}
