import { ChangeDetectionStrategy, Component, Input, OnInit } from '@angular/core'

@Component({
  selector: 'rcv-circle-chart',
  template: `
    <svg class="circle-chart" [attr.viewBox]="viewBox" [style.width.px]="size" [style.height.px]="size">
      <circle
        cx="50%"
        cy="50%"
        [attr.r]="radius"
        [style.stroke-dashoffset.px]="strokeDashOffset"
        [style.stroke-dasharray.px]="strokeCircumference"
      ></circle>
      <text x="49" y="-49" text-anchor="middle" alignment-baseline="middle">{{ value }}</text>
    </svg>
  `,
  styleUrls: ['./circle-chart.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class CircleChartComponent implements OnInit {
  @Input() size = 100
  @Input() value = 0
  @Input() domain = 100

  radius = 45

  get strokeCircumference(): number {
    return 2 * Math.PI * this.radius
  }

  get strokeDashOffset() {
    return (this.strokeCircumference * (this.domain - this.value)) / this.domain
  }

  get viewBox(): string {
    return `0 0 ${this.size} ${this.size}`
  }

  constructor() {}

  ngOnInit() {}
}
