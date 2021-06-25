import {
  RcvObjectDetectionSuggestResultInterface,
  RcvSingleImageObjectDetectionSuggestResultInterface,
} from '@rcv/domain/labeltool-client'

export type MultiSuggestionResult = RcvObjectDetectionSuggestResultInterface
export type SingleSuggestionResult = RcvSingleImageObjectDetectionSuggestResultInterface

export function isSingleSuggestionResult(arg: any): arg is SingleSuggestionResult {
  return arg.Labels != null
}
