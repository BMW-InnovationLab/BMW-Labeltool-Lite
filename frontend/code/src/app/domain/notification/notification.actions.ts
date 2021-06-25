export class SuccessNotification {
  static readonly type = '[NOTIFICATION] Notification'
  constructor(public message: string) {}
}
