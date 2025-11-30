import {ExhibitDetails, ExhibitsService} from '../../core/services/exhibit-service';
import {
  Component,
  computed,
  inject,
  input,
  numberAttribute,
  OnInit,
  Signal,
  signal,
  WritableSignal
} from '@angular/core';
import {customError, Field, form, max, min, required, submit, validate} from '@angular/forms/signals';
import {SnackbarService} from '../../core/services/snackbar-service';
import {MatError, MatFormField, MatInput, MatLabel} from '@angular/material/input';
import {MatButton} from '@angular/material/button';
import {MatCard, MatCardActions, MatCardContent, MatCardHeader, MatCardTitle} from '@angular/material/card';
import {MatProgressBar} from '@angular/material/progress-bar';
import {MatOption, MatSelect} from '@angular/material/select';
import {Router} from '@angular/router';
import {LocalDate} from '@js-joda/core';

@Component({
  selector: "app-exhibit-edit",
  imports: [
    MatFormField,
    MatInput,
    MatLabel,
    MatError,
    MatButton,
    MatCard,
    MatCardHeader,
    MatCardTitle,
    MatCardContent,
    MatCardActions,
    MatProgressBar,
    MatSelect,
    MatOption,
    Field
  ],
  templateUrl: "./exhibit-edit.html",
  styleUrl: "./exhibit-edit.scss"
})
export class ExhibitEdit implements OnInit {
  protected readonly MIN_YEAR: number = 1900;
  protected readonly MIN_CREW: number = 1;
  private readonly MAX_YEAR: number = new Date().getFullYear();
  private readonly service: ExhibitsService = inject(ExhibitsService);
  private readonly exhibitFormModel: WritableSignal<ExhibitFormModel> = signal({
    name: "",
    description: "",
    serviceStartYear: this.MIN_YEAR + 1,
    serviceEndYear: this.MAX_YEAR - 1,
    unitsProduced: 0,
    country: "",
    armor: "",
    armament: "",
    crew: 3,
    imageUrl: ""
  });
  protected readonly exhibitForm = form(this.exhibitFormModel, path => {
    required(path.name, {message: "Name is required"});
    required(path.description, {message: "Description is required"});
    required(path.serviceStartYear, {message: "Service start year is required"});
    min(path.serviceStartYear, this.MIN_YEAR, {message: `Cannot be before ${this.MIN_YEAR}`});
    max(path.serviceStartYear, this.MAX_YEAR, {message: `Cannot be after ${this.MAX_YEAR}`});
    min(path.serviceEndYear, this.MIN_YEAR, {message: `Cannot be before ${this.MIN_YEAR}`});
    max(path.serviceEndYear, this.MAX_YEAR, {message: `Cannot be after ${this.MAX_YEAR}`});
    required(path.unitsProduced, {message: "Units produced is required"});
    min(path.unitsProduced, 0, {message: "Units produced cannot be negative"});
    required(path.country, {message: "Country is required"});
    required(path.armor, {message: "Armor is required"});
    required(path.armament, {message: "Armament is required"});
    required(path.crew, {message: "Crew is required"});
    min(path.crew, this.MIN_CREW, {message: `Must be at least ${this.MIN_CREW}`});
    validate(path.imageUrl, ctx => {
      const urlPattern = /^https?:\/\/.+/;
      const value = ctx.valueOf(path.imageUrl);
      if (!value || urlPattern.test(value)) {
        // empty is fine as well
        return undefined;
      }

      return customError({
        kind: "imageUrlFormat",
        message: "Has to be a valid URL starting with http:// or https://"
      });
    });
  });
  private readonly snackbar: SnackbarService = inject(SnackbarService);
  protected readonly imageUrl: Signal<string | null> = this.exhibitForm.imageUrl().value.asReadonly();
  public readonly id = input<number | undefined, unknown>(undefined, {
    transform: (raw) => {
      if (raw == null) {
        return undefined;
      }
      return numberAttribute(raw);
    }
  });
  protected readonly editMode: Signal<boolean> = computed(() => {
    const id: number | undefined = this.id();
    return id != undefined && !isNaN(id) && id > 0;
  });
  private originallyAddedDate: LocalDate | undefined = undefined;
  private readonly router: Router = inject(Router);

  public async ngOnInit(): Promise<void> {
    if (!this.editMode()) {
      return;
    }

    const exhibitId = this.id();
    if (exhibitId === undefined || isNaN(exhibitId)) {
      this.snackbar.show("No exhibit ID provided for editing");
      return;
    }

    const exhibitDetails = await this.service.getExhibit(exhibitId);
    if (!exhibitDetails) {
      this.snackbar.show("Failed to load exhibit for editing");
      return;
    }

    this.exhibitFormModel.set({
      name: exhibitDetails.name,
      description: exhibitDetails.description,
      serviceStartYear: exhibitDetails.serviceStartYear,
      // NaN is a workaround until https://github.com/angular/angular/issues/65454 is fixed
      serviceEndYear: exhibitDetails.serviceEndYear || NaN,
      unitsProduced: exhibitDetails.unitsProduced,
      country: exhibitDetails.country,
      armor: exhibitDetails.armor,
      armament: exhibitDetails.armament,
      crew: exhibitDetails.crew,
      imageUrl: exhibitDetails.imageUrl || ""
    });

    this.originallyAddedDate = exhibitDetails.addedToMuseumCollectionAt;
  }

  public async handleFormSubmit(): Promise<void> {
    await submit(this.exhibitForm, async form => {
      const formValues = form().value();
      let result: ExhibitDetails | undefined | boolean;
      if (this.editMode()) {
        if (!this.originallyAddedDate) {
          this.snackbar.show("Cannot update exhibit, data missing");
          return;
        }
        const payload: ExhibitDetails = {
          id: this.id()!,
          addedToMuseumCollectionAt: this.originallyAddedDate,
          ...formValues
        };
        ExhibitEdit.ensureOptionalFormValuesAreNull(payload);
        result = await this.service.updateExhibit(payload);
      } else {
        ExhibitEdit.ensureOptionalFormValuesAreNull(formValues);
        result = await this.service.addExhibit(formValues);
      }

      const [presentVerb, pastVerb] = this.editMode()
        ? ["update", "updated"]
        : ["add", "added"];
      if (!result) {
        this.snackbar.show(`Failed to ${presentVerb} exhibit!`);
      } else {
        this.snackbar.show(`Exhibit ${pastVerb} successfully!`);
        if (this.editMode()) {
          await this.router.navigate(["/exhibits", this.id()]);
        } else {
          this.exhibitForm().reset();
        }
      }
    });
  }

  private static ensureOptionalFormValuesAreNull<T extends {
    serviceEndYear: number | null,
    imageUrl: string | null
  }>(formValues: T): T {
    // this method can most likely be removed once https://github.com/angular/angular/issues/65454 is fixed
    if (formValues.serviceEndYear !== null && isNaN(formValues.serviceEndYear)) {
      formValues.serviceEndYear = null;
    }
    if (formValues.imageUrl === "") {
      formValues.imageUrl = null;
    }
    return formValues;
  }
}

type ExhibitFormModel = {
  name: string;
  description: string;
  serviceStartYear: number;
  serviceEndYear: number;
  unitsProduced: number;
  country: string;
  armor: string;
  armament: string;
  crew: number;
  imageUrl: string;
};
