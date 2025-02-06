export type UserProfileType = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;

  aboutMe: string;

  activityStatus: string;
  casualStatus: string;
  moodStatus: string;
  workStatus: string;
  gamingStatus: string;

  friends: UserFriendType[];
};

export type UserFriendType = {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
};
