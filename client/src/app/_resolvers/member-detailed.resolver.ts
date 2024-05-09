import { ResolveFn } from '@angular/router';
import { Member } from '../_models/members';
import { Inject } from '@angular/core';
import { MembersService } from '../_services/members.service';

export const memberDetailedResolver: ResolveFn<Member> = (route, state) => {
  const memberService = Inject(MembersService);
  return memberService.getMember(route.paramMap.get('username')!);
};
