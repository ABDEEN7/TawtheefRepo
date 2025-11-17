import { Application } from "./application.model";
import { Invite } from "./invite.model";
import { Profile } from "./profile.model";

export interface InviteDetails extends Invite {
  profile: Profile;
  application?: Application;
}
