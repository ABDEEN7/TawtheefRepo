import { GUID } from "../../shared/types/guid.type";
export class GuidUtils {
  public static newGuid(): GUID {
    const guid = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, c => {
      const r = (Math.random() * 16) | 0;
      const v = c === 'x' ? r : (r & 0x3) | 0x8;
      return v.toString(16);
    });

    return guid as GUID;
  }

  public static readonly emptyGuid: GUID =
    "00000000-0000-0000-0000-000000000000" as GUID;

  public static readonly nullGuid: GUID | null = null;

  public static isValid(guid: string): boolean {
    const guidRegex =
      /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i;

    return guidRegex.test(guid);
  }

  public static asGuid(guid: string): GUID {
    if (!GuidUtils.isValid(guid)) {
      throw new Error("Invalid GUID format");
    }
    return guid as GUID;
  }
}
